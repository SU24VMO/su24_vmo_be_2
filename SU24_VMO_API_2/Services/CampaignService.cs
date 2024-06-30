using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.IdentityModel.Tokens;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Response;
using System.Diagnostics.Metrics;

namespace SU24_VMO_API.Services
{
    public class CampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignTypeRepository _campaignTypeRepository;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDonatePhaseRepository _donatePhaseRepository;
        private readonly IProcessingPhaseRepository _processingPhaseRepository;
        private readonly IStatementPhaseRepository _statementPhaseRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly FirebaseService _firebaseService;

        public CampaignService(ICampaignRepository campaignRepository, FirebaseService firebaseService, ICampaignTypeRepository campaignTypeRepository,
            ICreateCampaignRequestRepository createCampaignRequestRepository, IOrganizationRepository organizationRepository,
            IDonatePhaseRepository donatePhaseRepository, IProcessingPhaseRepository processingPhaseRepository, IStatementPhaseRepository statementPhaseRepository, 
            IUserRepository userRepository)
        {
            _campaignRepository = campaignRepository;
            _firebaseService = firebaseService;
            _campaignTypeRepository = campaignTypeRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _organizationRepository = organizationRepository;
            _donatePhaseRepository = donatePhaseRepository;
            _processingPhaseRepository = processingPhaseRepository;
            _statementPhaseRepository = statementPhaseRepository;
            _userRepository = userRepository;
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
            var campaign = _campaignRepository.GetById(campaignId);
            if(campaign != null && campaign.Organization != null && campaign.Organization.Campaigns != null)
            {
                campaign.Organization.Campaigns = null;
            }
            if(campaign != null && campaign.CampaignType != null && campaign.CampaignType.Campaigns != null)
            {
                campaign.CampaignType.Campaigns = null;
            }
            return campaign;
        }



        public IEnumerable<Campaign> GetAllCampaigns()
        {
            var campaigns = _campaignRepository.GetAll();
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;

            }
            return campaigns;
        }

        public IEnumerable<Campaign> GetAllCampaignsWithUnActiveStatus()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.IsActive == false);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }

        public IEnumerable<Campaign> GetAllCampaignsWithActiveStatus()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.IsActive == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }


        public IEnumerable<Campaign> GetAllCampaignsWithEndStatus()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.IsComplete == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }


        public IEnumerable<Campaign> GetAllCampaignsWithDonatePhaseWasEnd()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }

        public IEnumerable<Campaign> GetAllCampaignsByCampaignName(string campaignName)
        {
            var campaigns = _campaignRepository.GetCampaignsByCampaignName(campaignName);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }



        public IEnumerable<Campaign> GetAllCampaignsWithDonatePhaseIsProcessing()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.DonatePhase != null && c.DonatePhase.IsProcessing == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }

        public IEnumerable<Campaign> GetAllCampaignsWithProcessingPhaseIsProcessing()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.ProcessingPhase != null && c.ProcessingPhase.IsProcessing == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }


        public IEnumerable<Campaign> GetAllCampaignsWithStatementPhaseIsProcessing()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.StatementPhase != null && c.StatementPhase.IsProcessing == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }


        public IEnumerable<Campaign> GetAllCampaignsByCampaignTypeId(Guid campaignTypeId)
        {
            var campaigns = _campaignRepository.GetCampaignsByCampaignTypeId(campaignTypeId);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }


        public IEnumerable<Campaign> GetAllCampaignsByCampaignTypeIdWithStatus(Guid? campaignTypeId, string? status, string? campaignName, string? createBy)
        {
            //if (campaignTypeId != null)
            //{
            //    var campaigns = _campaignRepository.GetCampaignsByCampaignTypeId((Guid)campaignTypeId).Where(c => c.IsActive == true);
            //    foreach (var campaign in campaigns)
            //    {
            //        if (campaign.CampaignType != null)
            //            campaign.CampaignType!.Campaigns = null;
            //        if (campaign.Organization != null)
            //            campaign.Organization!.Campaigns = null;
            //    }

            //    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
            //    {
            //        campaigns = campaigns.Where(c => c.IsActive == true);
            //        foreach (var campaign in campaigns)
            //        {
            //            if (campaign.CampaignType != null)
            //                campaign.CampaignType!.Campaigns = null;
            //            if (campaign.Organization != null)
            //                campaign.Organization!.Campaigns = null;
            //        }
            //        return campaigns;
            //    }

            //    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
            //    {
            //        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
            //        foreach (var campaign in campaigns)
            //        {
            //            if (campaign.CampaignType != null)
            //                campaign.CampaignType!.Campaigns = null;
            //            if (campaign.Organization != null)
            //                campaign.Organization!.Campaigns = null;
            //        }
            //        return campaigns;
            //    }

            //    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
            //    {
            //        campaigns = campaigns.Where(c => c.IsComplete == true);
            //        foreach (var campaign in campaigns)
            //        {
            //            if (campaign.CampaignType != null)
            //                campaign.CampaignType!.Campaigns = null;
            //            if (campaign.Organization != null)
            //                campaign.Organization!.Campaigns = null;
            //        }
            //        return campaigns;
            //    }
            //    return campaigns;
            //}
            //else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
            //{
            //    var campaigns = GetAllCampaignsWithActiveStatus();
            //    foreach (var campaign in campaigns)
            //    {
            //        if (campaign.CampaignType != null)
            //            campaign.CampaignType!.Campaigns = null;
            //        if (campaign.Organization != null)
            //            campaign.Organization!.Campaigns = null;
            //    }
            //    return campaigns;
            //}
            //else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
            //{
            //    var campaigns = GetAllCampaignsWithDonatePhaseWasEnd();
            //    foreach (var campaign in campaigns)
            //    {
            //        if (campaign.CampaignType != null)
            //            campaign.CampaignType!.Campaigns = null;
            //        if (campaign.Organization != null)
            //            campaign.Organization!.Campaigns = null;
            //    }
            //    return campaigns;
            //}
            //else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
            //{
            //    var campaigns = GetAllCampaignsWithEndStatus();
            //    foreach (var campaign in campaigns)
            //    {
            //        if (campaign.CampaignType != null)
            //            campaign.CampaignType!.Campaigns = null;
            //        if (campaign.Organization != null)
            //            campaign.Organization!.Campaigns = null;
            //    }
            //    return campaigns;
            //}
            //return GetAllCampaignsWithActiveStatus();
            if (!String.IsNullOrEmpty(createBy) && createBy.ToLower().Equals("organization"))
            {
                var campaigns = GetAllCampaigns().Where(c => c.IsActive == true && c.OrganizationID != null);
                if (campaignTypeId != null)
                {
                    campaigns = campaigns.Where(c => c.IsActive == true && c.CampaignTypeID.Equals(campaignTypeId));
                    foreach (var campaign in campaigns)
                    {
                        if (campaign.CampaignType != null)
                            campaign.CampaignType!.Campaigns = null;
                        if (campaign.Organization != null)
                            campaign.Organization!.Campaigns = null;
                    }
                    if (!String.IsNullOrEmpty(campaignName))
                    {
                        campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                    }
                    return campaigns;
                }
                else if (!String.IsNullOrEmpty(status))
                {
                    if (campaignTypeId != null)
                    {
                        campaigns = campaigns.Where(c => c.IsActive == true && c.CampaignTypeID.Equals(campaignTypeId));
                        if (!String.IsNullOrEmpty(campaignName))
                        {
                            campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                        }
                        return campaigns;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(campaignName))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true && c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()));
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            return campaigns;
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            else
                            {
                                return campaigns.Where(c => c.IsActive == true);
                            }
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(campaignName))
                {
                    campaigns = campaigns.Where(c => c.IsActive == true && c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()));
                    if (campaignTypeId != null)
                    {
                        campaigns = campaigns.Where(c => c.IsActive == true && campaignTypeId.Equals(campaignTypeId));
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                        return campaigns;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                        return campaigns;
                    }
                }
                else
                {
                    return campaigns.Where(c => c.IsActive == true);
                }
            }
            else if (!String.IsNullOrEmpty(createBy) && createBy.ToLower().Equals("member"))
            {
                var campaigns = GetAllCampaigns().Where(c => c.IsActive == true && c.OrganizationID == null);
                if (campaignTypeId != null)
                {
                    campaigns = campaigns.Where(c => c.IsActive == true && c.CampaignTypeID.Equals(campaignTypeId));
                    foreach (var campaign in campaigns)
                    {
                        if (campaign.CampaignType != null)
                            campaign.CampaignType!.Campaigns = null;
                        if (campaign.Organization != null)
                            campaign.Organization!.Campaigns = null;
                    }
                    if (!String.IsNullOrEmpty(campaignName))
                    {
                        campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                    }
                    return campaigns;
                }
                else if (!String.IsNullOrEmpty(status))
                {
                    if (campaignTypeId != null)
                    {
                        campaigns = campaigns.Where(c => c.IsActive == true && c.CampaignTypeID.Equals(campaignTypeId));
                        if (!String.IsNullOrEmpty(campaignName))
                        {
                            campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                        }
                        return campaigns;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(campaignName))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true && c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()));
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            return campaigns;
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            else
                            {
                                return campaigns.Where(c => c.IsActive == true);
                            }
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(campaignName))
                {
                    campaigns = campaigns.Where(c => c.IsActive == true && c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()));
                    if (campaignTypeId != null)
                    {
                        campaigns = campaigns.Where(c => c.IsActive == true && campaignTypeId.Equals(campaignTypeId));
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                        return campaigns;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                        return campaigns;
                    }
                }
                else
                {
                    return campaigns.Where(c => c.IsActive == true);
                }
            }
            else
            {
                if (campaignTypeId != null)
                {
                    var campaigns = _campaignRepository.GetCampaignsByCampaignTypeId((Guid)campaignTypeId).Where(c => c.IsActive == true);
                    foreach (var campaign in campaigns)
                    {
                        if (campaign.CampaignType != null)
                            campaign.CampaignType!.Campaigns = null;
                        if (campaign.Organization != null)
                            campaign.Organization!.Campaigns = null;
                    }
                    if (!String.IsNullOrEmpty(campaignName))
                    {
                        campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                    }
                    return campaigns;
                }
                else if (!String.IsNullOrEmpty(status))
                {
                    if (campaignTypeId != null)
                    {
                        var campaigns = _campaignRepository.GetCampaignsByCampaignTypeId((Guid)campaignTypeId).Where(c => c.IsActive == true);
                        if (!String.IsNullOrEmpty(campaignName))
                        {
                            campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                        }
                        return campaigns;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(campaignName))
                        {
                            var campaigns = GetAllCampaignsByCampaignName(campaignName).Where(c => c.IsActive == true);
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsActive == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }

                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                campaigns = campaigns.Where(c => c.IsComplete == true);
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            return campaigns;
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                            {
                                var campaigns = GetAllCampaignsWithActiveStatus();
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                            {
                                var campaigns = GetAllCampaignsWithDonatePhaseWasEnd();
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                            {
                                var campaigns = GetAllCampaignsWithEndStatus();
                                foreach (var campaign in campaigns)
                                {
                                    if (campaign.CampaignType != null)
                                        campaign.CampaignType!.Campaigns = null;
                                    if (campaign.Organization != null)
                                        campaign.Organization!.Campaigns = null;
                                }
                                return campaigns;
                            }
                            else
                            {
                                return GetAllCampaigns().Where(c => c.IsActive == true);
                            }
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(campaignName))
                {
                    var campaigns = GetAllCampaignsByCampaignName(campaignName).Where(c => c.IsActive == true);
                    if (campaignTypeId != null)
                    {
                        campaigns = campaigns.Where(c => c.IsActive == true && campaignTypeId.Equals(campaignTypeId));
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                        return campaigns;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsActive == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }

                        if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
                        {
                            campaigns = campaigns.Where(c => c.IsComplete == true);
                            foreach (var campaign in campaigns)
                            {
                                if (campaign.CampaignType != null)
                                    campaign.CampaignType!.Campaigns = null;
                                if (campaign.Organization != null)
                                    campaign.Organization!.Campaigns = null;
                            }
                            return campaigns;
                        }
                        return campaigns;
                    }
                }
                else
                {
                    return GetAllCampaigns().Where(c => c.IsActive == true);
                }
            }
        }


        public IEnumerable<Campaign> GetAllCampaignsWithActiveStatusByCampaignTypeId(Guid campaignTypeId)
        {
            var campaigns = _campaignRepository.GetCampaignsByCampaignTypeId(campaignTypeId).Where(c => c.IsActive == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }

        public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerId(Guid organizationManagerId)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
            var campaigns = new List<Campaign>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                    {
                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                        createCampaignRequest.Campaign!.Organization = organization;
                        campaigns.Add(createCampaignRequest.Campaign!);
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
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

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }



        public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerIdWithDonatePhaseIsProcessing(Guid organizationManagerId)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
            var campaigns = new List<Campaign>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                    {
                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                        createCampaignRequest.Campaign!.Organization = organization;
                        var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
                        if (donatePhase != null && donatePhase.IsProcessing == true)
                        {
                            createCampaignRequest.Campaign!.Organization = organization;
                            createCampaignRequest.Campaign!.DonatePhase = donatePhase;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.DonatePhase != null)
                    campaign.DonatePhase.Campaign = null;
            }
            return campaigns;
        }


        public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerIdWithOptionsPhaseInProcessingPhase(Guid organizationManagerId, string? status)
        {
            if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("donate-phase".ToLower()))
            {
                var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                        {
                            var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                            createCampaignRequest.Campaign!.Organization = organization;
                            var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
                            if (donatePhase != null && donatePhase.IsProcessing == true)
                            {
                                createCampaignRequest.Campaign!.Organization = organization;
                                createCampaignRequest.Campaign!.DonatePhase = donatePhase;
                                campaigns.Add(createCampaignRequest.Campaign!);
                            }
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.DonatePhase != null)
                        campaign.DonatePhase.Campaign = null;
                }
                return campaigns;
            }
            else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("processing-phase".ToLower()))
            {
                var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                        {
                            var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                            createCampaignRequest.Campaign!.Organization = organization;
                            var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
                            if (processingPhase != null && processingPhase.IsProcessing == true)
                            {
                                createCampaignRequest.Campaign!.Organization = organization;
                                createCampaignRequest.Campaign!.ProcessingPhase = processingPhase;
                                campaigns.Add(createCampaignRequest.Campaign!);
                            }
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.DonatePhase != null)
                        campaign.DonatePhase.Campaign = null;
                }
                return campaigns;
            }
            else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("statement-phase".ToLower()))
            {
                var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                        {
                            var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                            createCampaignRequest.Campaign!.Organization = organization;
                            var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
                            if (statementPhase != null && statementPhase.IsProcessing == true)
                            {
                                createCampaignRequest.Campaign!.Organization = organization;
                                createCampaignRequest.Campaign!.StatementPhase = statementPhase;
                                campaigns.Add(createCampaignRequest.Campaign!);
                            }
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.DonatePhase != null)
                        campaign.DonatePhase.Campaign = null;
                }
                return campaigns;
            }
            else
            {
                throw new InvalidOperationException("Phase is invalid!");
            }
        }


        public IEnumerable<CampaignResponse?> GetAllCampaignByCreateByMemberIdWithOptionsPhaseInProcessingPhase(Guid userId, string? status)
        {
            if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("donate-phase".ToLower()))
            {
                var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByMemberId(userId);
                var campaigns = new List<CampaignResponse>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        var member = _userRepository.GetById(userId);
                        var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
                        if (donatePhase != null && donatePhase.IsProcessing == true)
                        {
                            createCampaignRequest.Campaign!.DonatePhase = donatePhase;
                            campaigns.Add(new CampaignResponse
                            {
                                CampaignID = createCampaignRequest.Campaign!.CampaignID,
                                OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
                                CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
                                ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
                                Address = createCampaignRequest.Campaign!.Address,
                                ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
                                CampaignType = createCampaignRequest.Campaign!.CampaignType,
                                CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
                                CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
                                CreateAt = createCampaignRequest.Campaign!.CreateAt,
                                Description = createCampaignRequest.Campaign!.Description,
                                DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
                                ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
                                Image = createCampaignRequest.Campaign!.Image,
                                IsActive = createCampaignRequest.Campaign!.IsActive,
                                IsComplete = createCampaignRequest.Campaign!.IsComplete,
                                IsModify = createCampaignRequest.Campaign!.IsModify,
                                IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
                                Name = createCampaignRequest.Campaign!.Name,
                                Note = createCampaignRequest.Campaign!.Note,
                                Organization = createCampaignRequest.Campaign!.Organization,
                                ProcessingPhase = createCampaignRequest.Campaign!.ProcessingPhase,
                                StartDate = createCampaignRequest.Campaign!.StartDate,
                                StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
                                TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
                                Transactions = createCampaignRequest.Campaign!.Transactions,
                                UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
                                User = member
                            });
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.DonatePhase != null)
                        campaign.DonatePhase.Campaign = null;
                }
                return campaigns;
            }
            else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("processing-phase".ToLower()))
            {
                var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByMemberId(userId);
                var campaigns = new List<CampaignResponse>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        var member = _userRepository.GetById(userId);
                        var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
                        if (processingPhase != null && processingPhase.IsProcessing == true)
                        {
                            campaigns.Add(new CampaignResponse
                            {
                                CampaignID = createCampaignRequest.Campaign!.CampaignID,
                                OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
                                CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
                                ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
                                Address = createCampaignRequest.Campaign!.Address,
                                ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
                                CampaignType = createCampaignRequest.Campaign!.CampaignType,
                                CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
                                CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
                                CreateAt = createCampaignRequest.Campaign!.CreateAt,
                                Description = createCampaignRequest.Campaign!.Description,
                                DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
                                ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
                                Image = createCampaignRequest.Campaign!.Image,
                                IsActive = createCampaignRequest.Campaign!.IsActive,
                                IsComplete = createCampaignRequest.Campaign!.IsComplete,
                                IsModify = createCampaignRequest.Campaign!.IsModify,
                                IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
                                Name = createCampaignRequest.Campaign!.Name,
                                Note = createCampaignRequest.Campaign!.Note,
                                Organization = createCampaignRequest.Campaign!.Organization,
                                ProcessingPhase = createCampaignRequest.Campaign!.ProcessingPhase,
                                StartDate = createCampaignRequest.Campaign!.StartDate,
                                StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
                                TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
                                Transactions = createCampaignRequest.Campaign!.Transactions,
                                UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
                                User = member
                            });
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.DonatePhase != null)
                        campaign.DonatePhase.Campaign = null;
                }
                return campaigns;
            }
            else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("statement-phase".ToLower()))
            {
                var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByMemberId(userId);
                var campaigns = new List<CampaignResponse>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        var member = _userRepository.GetById(userId);
                        var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
                        if (statementPhase != null && statementPhase.IsProcessing == true)
                        {
                            campaigns.Add(new CampaignResponse
                            {
                                CampaignID = createCampaignRequest.Campaign!.CampaignID,
                                OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
                                CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
                                ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
                                Address = createCampaignRequest.Campaign!.Address,
                                ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
                                CampaignType = createCampaignRequest.Campaign!.CampaignType,
                                CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
                                CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
                                CreateAt = createCampaignRequest.Campaign!.CreateAt,
                                Description = createCampaignRequest.Campaign!.Description,
                                DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
                                ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
                                Image = createCampaignRequest.Campaign!.Image,
                                IsActive = createCampaignRequest.Campaign!.IsActive,
                                IsComplete = createCampaignRequest.Campaign!.IsComplete,
                                IsModify = createCampaignRequest.Campaign!.IsModify,
                                IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
                                Name = createCampaignRequest.Campaign!.Name,
                                Note = createCampaignRequest.Campaign!.Note,
                                Organization = createCampaignRequest.Campaign!.Organization,
                                ProcessingPhase = createCampaignRequest.Campaign!.ProcessingPhase,
                                StartDate = createCampaignRequest.Campaign!.StartDate,
                                StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
                                TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
                                Transactions = createCampaignRequest.Campaign!.Transactions,
                                UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
                                User = member
                            });
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.DonatePhase != null)
                        campaign.DonatePhase.Campaign = null;
                }
                return campaigns;
            }
            else
            {
                throw new InvalidOperationException("Phase is invalid!");
            }
        }



        public IEnumerable<CampaignResponse?> GetAllCampaignByCreateByMemberIdWithDonatePhaseIsProcessing(Guid userId)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByMemberId(userId);
            var campaigns = new List<CampaignResponse>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    var member = _userRepository.GetById(userId);
                    var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
                    if (donatePhase != null && donatePhase.IsProcessing == true)
                    {
                        campaigns.Add(new CampaignResponse
                        {
                            CampaignID = createCampaignRequest.Campaign!.CampaignID,
                            OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
                            CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
                            ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
                            Address = createCampaignRequest.Campaign!.Address,
                            ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
                            CampaignType = createCampaignRequest.Campaign!.CampaignType,
                            CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
                            CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
                            CreateAt = createCampaignRequest.Campaign!.CreateAt,
                            Description = createCampaignRequest.Campaign!.Description,
                            DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
                            ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
                            Image = createCampaignRequest.Campaign!.Image,
                            IsActive = createCampaignRequest.Campaign!.IsActive,
                            IsComplete = createCampaignRequest.Campaign!.IsComplete,
                            IsModify = createCampaignRequest.Campaign!.IsModify,
                            IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
                            Name = createCampaignRequest.Campaign!.Name,
                            Note = createCampaignRequest.Campaign!.Note,
                            Organization = createCampaignRequest.Campaign!.Organization,
                            ProcessingPhase = createCampaignRequest.Campaign!.ProcessingPhase,
                            StartDate = createCampaignRequest.Campaign!.StartDate,
                            StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
                            TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
                            Transactions = createCampaignRequest.Campaign!.Transactions,
                            UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
                            User = member
                        });
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
            }
            return campaigns;
        }


        public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerIdWithProcessingPhaseIsProcessing(Guid organizationManagerId)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
            var campaigns = new List<Campaign>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                    {
                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                        createCampaignRequest.Campaign!.Organization = organization;
                        var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
                        if (processingPhase != null && processingPhase.IsProcessing == true)
                        {
                            createCampaignRequest.Campaign!.Organization = organization;
                            createCampaignRequest.Campaign!.ProcessingPhase = processingPhase;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.DonatePhase != null)
                    campaign.DonatePhase.Campaign = null;
            }
            return campaigns;
        }


        public IEnumerable<CampaignResponse?> GetAllCampaignByCreateByMemberIdWithProcessingPhaseIsProcessing(Guid userId)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByMemberId(userId);
            var campaigns = new List<CampaignResponse>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    var member = _userRepository.GetById(userId);
                    var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
                    if (processingPhase != null && processingPhase.IsProcessing == true)
                    {
                        campaigns.Add(new CampaignResponse
                        {
                            CampaignID = createCampaignRequest.Campaign!.CampaignID,
                            OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
                            CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
                            ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
                            Address = createCampaignRequest.Campaign!.Address,
                            ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
                            CampaignType = createCampaignRequest.Campaign!.CampaignType,
                            CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
                            CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
                            CreateAt = createCampaignRequest.Campaign!.CreateAt,
                            Description = createCampaignRequest.Campaign!.Description,
                            DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
                            ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
                            Image = createCampaignRequest.Campaign!.Image,
                            IsActive = createCampaignRequest.Campaign!.IsActive,
                            IsComplete = createCampaignRequest.Campaign!.IsComplete,
                            IsModify = createCampaignRequest.Campaign!.IsModify,
                            IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
                            Name = createCampaignRequest.Campaign!.Name,
                            Note = createCampaignRequest.Campaign!.Note,
                            Organization = createCampaignRequest.Campaign!.Organization,
                            ProcessingPhase = createCampaignRequest.Campaign!.ProcessingPhase,
                            StartDate = createCampaignRequest.Campaign!.StartDate,
                            StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
                            TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
                            Transactions = createCampaignRequest.Campaign!.Transactions,
                            UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
                            User = member
                        });
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.DonatePhase != null)
                    campaign.DonatePhase.Campaign = null;
            }
            return campaigns;
        }




        public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerIdWithStatementIsProcessing(Guid organizationManagerId)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
            var campaigns = new List<Campaign>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                    {
                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                        createCampaignRequest.Campaign!.Organization = organization;
                        var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
                        if (statementPhase != null && statementPhase.IsProcessing == true)
                        {
                            createCampaignRequest.Campaign!.Organization = organization;
                            createCampaignRequest.Campaign!.StatementPhase = statementPhase;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.DonatePhase != null)
                    campaign.DonatePhase.Campaign = null;
            }
            return campaigns;
        }



        public IEnumerable<CampaignResponse?> GetAllCampaignByCreateByMemberIdWithStatementPhaseIsProcessing(Guid userId)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByMemberId(userId);
            var campaigns = new List<CampaignResponse>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    var member = _userRepository.GetById(userId);
                    var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
                    if (statementPhase != null && statementPhase.IsProcessing == true)
                    {
                        campaigns.Add(new CampaignResponse
                        {
                            CampaignID = createCampaignRequest.Campaign!.CampaignID,
                            OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
                            CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
                            ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
                            Address = createCampaignRequest.Campaign!.Address,
                            ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
                            CampaignType = createCampaignRequest.Campaign!.CampaignType,
                            CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
                            CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
                            CreateAt = createCampaignRequest.Campaign!.CreateAt,
                            Description = createCampaignRequest.Campaign!.Description,
                            DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
                            ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
                            Image = createCampaignRequest.Campaign!.Image,
                            IsActive = createCampaignRequest.Campaign!.IsActive,
                            IsComplete = createCampaignRequest.Campaign!.IsComplete,
                            IsModify = createCampaignRequest.Campaign!.IsModify,
                            IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
                            Name = createCampaignRequest.Campaign!.Name,
                            Note = createCampaignRequest.Campaign!.Note,
                            Organization = createCampaignRequest.Campaign!.Organization,
                            ProcessingPhase = createCampaignRequest.Campaign!.ProcessingPhase,
                            StartDate = createCampaignRequest.Campaign!.StartDate,
                            StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
                            TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
                            Transactions = createCampaignRequest.Campaign!.Transactions,
                            UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
                            User = member
                        });
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
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
