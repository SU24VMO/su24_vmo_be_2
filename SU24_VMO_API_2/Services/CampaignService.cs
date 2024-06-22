﻿using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.IdentityModel.Tokens;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class CampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignTypeRepository _campaignTypeRepository;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly FirebaseService _firebaseService;

        public CampaignService(ICampaignRepository campaignRepository, FirebaseService firebaseService, ICampaignTypeRepository campaignTypeRepository,
            ICreateCampaignRequestRepository createCampaignRequestRepository, IOrganizationRepository organizationRepository)
        {
            _campaignRepository = campaignRepository;
            _firebaseService = firebaseService;
            _campaignTypeRepository = campaignTypeRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _organizationRepository = organizationRepository;
        }

        public async void UpdateCampaignRequest(Guid campaignId, UpdateCampaignRequest request)
        {
            var campaign = _campaignRepository.GetById(campaignId);
            if (campaign == null)
            {
                throw new Exception("Campaign not found!");
            }

            if (!String.IsNullOrEmpty(request.Name))
            {
                campaign.Name = request.Name;
            }

            if (!String.IsNullOrEmpty(request.Address))
            {
                campaign.Address = request.Address;
            }
            if (!String.IsNullOrEmpty(request.Description))
            {
                campaign.Description = request.Description;
            }
            if (request.Image != null)
            {
                var image = await _firebaseService.UploadImage(request.Image);
                campaign.Image = image;
            }
            if (!String.IsNullOrEmpty(request.ExpectedEndDate.ToString()))
            {
                campaign.ExpectedEndDate = (DateTime)request.ExpectedEndDate!;
            }
            if (!String.IsNullOrEmpty(request.ApplicationConfirmForm))
            {
                campaign.ApplicationConfirmForm = request.ApplicationConfirmForm!;
            }
            if (!String.IsNullOrEmpty(request.Note))
            {
                campaign.Note = request.Note!;
            }
            if (!String.IsNullOrEmpty(request.IsTransparent.ToString()))
            {
                campaign.IsTransparent = (bool)request.IsTransparent!;
            }
            if (!String.IsNullOrEmpty(request.IsModify.ToString()))
            {
                campaign.IsModify = (bool)request.IsModify!;
            }
            if (!String.IsNullOrEmpty(request.IsComplete.ToString()))
            {
                campaign.IsComplete = (bool)request.IsComplete!;
            }

            if (!String.IsNullOrEmpty(request.CanBeDonated.ToString()))
            {
                campaign.CanBeDonated = (bool)request.CanBeDonated!;
            }
            campaign.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
            _campaignRepository.Update(campaign);
        }
        public void UpdateCampaign(Campaign campaign)
        {
            _campaignRepository.Update(campaign);

        }


        public Campaign? GetCampaignByCampaignId(Guid campaignId)
        {
            return _campaignRepository.GetById(campaignId);
        }

        public IEnumerable<Campaign> GetAllCampaigns()
        {
            return _campaignRepository.GetAll();
        }

        public IEnumerable<Campaign> GetAllCampaignsByCampaignName(string campaignName)
        {
            return _campaignRepository.GetCampaignsByCampaignName(campaignName);
        }

        public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerId(Guid organizationManagerId)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
            var campaigns = new List<Campaign>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                    createCampaignRequest.Campaign!.Organization = organization;
                    campaigns.Add(createCampaignRequest.Campaign!);
                }
            }
            return campaigns;
        }
        public IEnumerable<Campaign?> GetAllCampaignByCreateByMemberId(Guid userId)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByMemberId(userId);
            var campaigns = new List<Campaign>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    campaigns.Add(createCampaignRequest.Campaign!);
                }
            }
            return campaigns;
        }

        public async Task<Campaign?> CreateCampaign(CreateNewCampaignRequest request)
        {
            TryValidateCreateCampaignRequest(request);
            //var createCampaignRequest = new CreateCampaignRequest();
            //if (request.CreateByOM != null)
            //{
            //    createCampaignRequest = new CreateCampaignRequest
            //    {
            //        CreateCampaignRequestID = Guid.NewGuid(),
            //        CampaignID = request.CampaignId,
            //        CreateByOM = request.CreateByOM,
            //        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
            //        IsApproved = false,
            //        IsPending = true,
            //        IsRejected = false,
            //        IsLocked = false,
            //    };
            //}
            //else
            //{
            //    createCampaignRequest = new CreateCampaignRequest
            //    {
            //        CreateCampaignRequestID = Guid.NewGuid(),
            //        CampaignID = request.CampaignId,
            //        CreateByUser = request.CreateByUser,
            //        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
            //        IsApproved = false,
            //        IsPending = true,
            //        IsRejected = false,
            //        IsLocked = false,
            //    };
            //}
            //_create.Save(createCampaignRequest);
            var campaign = new Campaign();
            if (request.OrganizationID == null)
            {
                campaign = new Campaign
                {
                    CampaignID = Guid.NewGuid(),
                    CampaignTypeID = request.CampaignTypeId,
                    Address = request.Address,
                    ApplicationConfirmForm = request.ApplicationConfirmForm,
                    Name = request.Name,
                    Description = request.Description,
                    Image = await _firebaseService.UploadImage(request.ImageCampaign),
                    StartDate = request.StartDate,
                    ExpectedEndDate = request.ExpectedEndDate,
                    TargetAmount = request.TargetAmount,
                    IsTransparent = false,
                    CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                    IsActive = false,
                    IsModify = false,
                    IsComplete = false,
                    CanBeDonated = false,
                };
                return _campaignRepository.Save(campaign);
            }
            else
            {
                campaign = new Campaign
                {
                    CampaignID = Guid.NewGuid(),
                    CampaignTypeID = request.CampaignTypeId,
                    OrganizationID = request.OrganizationID,
                    Address = request.Address,
                    ApplicationConfirmForm = request.ApplicationConfirmForm,
                    Name = request.Name,
                    Description = request.Description,
                    Image = await _firebaseService.UploadImage(request.ImageCampaign),
                    StartDate = request.StartDate,
                    ExpectedEndDate = request.ExpectedEndDate,
                    TargetAmount = request.TargetAmount,
                    IsTransparent = false,
                    CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                    IsActive = false,
                    IsModify = false,
                    IsComplete = false,
                    CanBeDonated = false,
                };
                return _campaignRepository.Save(campaign);
            }

        }


        private void TryValidateCreateCampaignRequest(CreateNewCampaignRequest request)
        {
            if (!String.IsNullOrEmpty(request.CampaignTypeId.ToString()))
            {
                throw new Exception("CampaignTypeId must not empty.");
            }

            if (_campaignTypeRepository.GetById(request.CampaignTypeId) == null)
            {
                throw new Exception("CampaignType not found.");
            }
            if (!String.IsNullOrEmpty(request.Name))
            {
                throw new Exception("Name is not empty.");
            }
            if (!String.IsNullOrEmpty(request.Address))
            {
                throw new Exception("Address is not empty.");
            }
            if (!String.IsNullOrEmpty(request.Description))
            {
                throw new Exception("Description is not empty.");
            }
            if (!String.IsNullOrEmpty(request.StartDate.ToString()))
            {
                throw new Exception("StartDate is not empty.");
            }
            if (!String.IsNullOrEmpty(request.TargetAmount))
            {
                throw new Exception("TargetAmount is not empty.");
            }
            if (!String.IsNullOrEmpty(request.ApplicationConfirmForm))
            {
                throw new Exception("ApplicationConfirmForm is not empty.");
            }
            if (!String.IsNullOrEmpty(request.TargetAmount))
            {
                throw new Exception("TargetAmount is not empty.");
            }
            if (!String.IsNullOrEmpty(request.AccountName))
            {
                throw new Exception("AccountName is not empty.");
            }
            if (request.StartDate < TimeHelper.GetTime(DateTime.UtcNow))
            {
                throw new Exception("Start date must greater than current time!");
            }

            if (request.ExpectedEndDate < TimeHelper.GetTime(DateTime.UtcNow))
            {
                throw new Exception("End date must greater than current time!");
            }

            if (request.StartDate > request.ExpectedEndDate)
            {
                throw new Exception("End date must greater than start date!");
            }

            if (Convert.ToInt64(request.TargetAmount) < 0)
            {
                throw new Exception("Target amount must greater or equal than 0.");
            }
        }


    }
}