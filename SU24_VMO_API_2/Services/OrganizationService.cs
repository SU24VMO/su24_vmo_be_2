using BusinessObject.Models;
using HtmlAgilityPack;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X509;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Request;

namespace SU24_VMO_API.Services
{
    public class OrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly ICreateOrganizationRequestRepository _createOrganizationRequestRepository;
        private readonly FirebaseService _firebaseService;
        private readonly IAccountRepository _accountRepository;

        public OrganizationService(IOrganizationRepository organizationRepository, IOrganizationManagerRepository organizationManagerRepository,
            IAccountRepository accountRepository, FirebaseService firebaseService, ICreateOrganizationRequestRepository createOrganizationRequestRepository)
        {
            _organizationRepository = organizationRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _accountRepository = accountRepository;
            _firebaseService = firebaseService;
            _createOrganizationRequestRepository = createOrganizationRequestRepository;
        }

        public IEnumerable<Organization> GetAllOrganizations()
        {
            var organizations = _organizationRepository.GetAll();
            foreach (var organization in organizations)
            {
                if (organization.Campaigns != null)
                {
                    foreach (var campaign in organization.Campaigns)
                    {
                        campaign.Transactions = null;
                        campaign.Organization = null;
                        campaign.CampaignType = null;
                        campaign.ProcessingPhase = null;
                        campaign.DonatePhase = null;
                        campaign.StatementPhase = null;
                    }
                }
                if (organization.Achievements != null)
                {
                    organization.Achievements.Clear();
                }
                if (organization.OrganizationManager != null)
                {
                    organization.OrganizationManager.Organizations = null;
                }

                if (organization.OrganizationManager != null)
                {
                    var account = _accountRepository.GetById(organization.OrganizationManager.AccountID);
                    if (account != null)
                    {
                        account.Notifications = null;
                        account.BankingAccounts = null;
                        account.AccountTokens = null;
                        account.Transactions = null;
                    }
                    organization.OrganizationManager.Account = account;

                    if (organization.OrganizationManager.CreateCampaignRequests != null)
                        organization.OrganizationManager.CreateCampaignRequests.Clear();
                    if (organization.OrganizationManager.CreateOrganizationRequests != null)
                        organization.OrganizationManager.CreateOrganizationRequests.Clear();
                    if (organization.OrganizationManager.CreatePostRequests != null)
                        organization.OrganizationManager.CreatePostRequests.Clear();
                }
            }
            return organizations;
        }


        public Organization? GetOrganizationByOrganizationId(Guid organizationId)
        {
            return GetAllOrganizations().FirstOrDefault(o => o.OrganizationID.Equals(organizationId));
        }

        public IEnumerable<Organization> GetAllOrganizationsByOrganizationName(string? organizationName)
        {
            if (!String.IsNullOrEmpty(organizationName))
            {
                var organizations = _organizationRepository.GetOrganizationsByOrganizationName(organizationName);
                foreach (var organization in organizations)
                {
                    if (organization.Campaigns != null)
                    {
                        foreach (var campaign in organization.Campaigns)
                        {
                            campaign.Transactions = null;
                            campaign.Organization = null;
                            campaign.CampaignType = null;
                            campaign.ProcessingPhase = null;
                            campaign.DonatePhase = null;
                            campaign.StatementPhase = null;
                        }
                    }
                    if (organization.Achievements != null)
                    {
                        organization.Achievements.Clear();
                    }
                    if (organization.OrganizationManager != null)
                    {
                        organization.OrganizationManager.Organizations = null;
                    }

                    if (organization.OrganizationManager != null)
                    {
                        if (organization.OrganizationManager.CreateCampaignRequests != null)
                            organization.OrganizationManager.CreateCampaignRequests.Clear();
                        if (organization.OrganizationManager.CreateOrganizationRequests != null)
                            organization.OrganizationManager.CreateOrganizationRequests.Clear();
                        if (organization.OrganizationManager.CreatePostRequests != null)
                            organization.OrganizationManager.CreatePostRequests.Clear();
                    }
                }
                return organizations;
            }
            else return GetAllOrganizations();


        }
        public IEnumerable<Organization> GetAllOrganizationsByOrganizationManagerId(Guid organizationManagerId, string? organizationName)
        {
            if (!String.IsNullOrEmpty(organizationName))
            {
                var organizations = _organizationRepository.GetAllOrganizationsByOrganizationManagerId(organizationManagerId);
                foreach (var organization in organizations)
                {
                    if (organization.Campaigns != null)
                    {
                        organization.Campaigns.Clear();
                    }
                    if (organization.Achievements != null)
                    {
                        organization.Achievements.Clear();
                    }
                    if (organization.OrganizationManager != null)
                    {
                        organization.OrganizationManager.Organizations = null;
                    }

                    if (organization.OrganizationManager != null)
                    {
                        if (organization.OrganizationManager.CreateCampaignRequests != null)
                            organization.OrganizationManager.CreateCampaignRequests.Clear();
                        if (organization.OrganizationManager.CreateOrganizationRequests != null)
                            organization.OrganizationManager.CreateOrganizationRequests.Clear();
                        if (organization.OrganizationManager.CreatePostRequests != null)
                            organization.OrganizationManager.CreatePostRequests.Clear();
                    }

                    var createOrganizationRequest =
                        _createOrganizationRequestRepository.GetCreateOrganizationRequestByOrganizationId(organization
                            .OrganizationID);
                    if (createOrganizationRequest != null)
                    {
                        createOrganizationRequest.Organization = null;
                        createOrganizationRequest.OrganizationManager = null;
                        createOrganizationRequest.Moderator = null;
                        organization.CreateOrganizationRequest = createOrganizationRequest;
                    }


                }
                return organizations.Where(o => o.Name.ToLower().Contains(organizationName.Trim().ToLower()) && o.IsDisable == false);
            }
            else
            {
                var organizations = _organizationRepository.GetAllOrganizationsByOrganizationManagerId(organizationManagerId);
                foreach (var organization in organizations)
                {
                    if (organization.Campaigns != null)
                    {
                        organization.Campaigns.Clear();
                    }
                    if (organization.Achievements != null)
                    {
                        organization.Achievements.Clear();
                    }
                    if (organization.OrganizationManager != null)
                    {
                        organization.OrganizationManager.Organizations = null;
                    }

                    if (organization.OrganizationManager != null)
                    {
                        if (organization.OrganizationManager.CreateCampaignRequests != null)
                            organization.OrganizationManager.CreateCampaignRequests.Clear();
                        if (organization.OrganizationManager.CreateOrganizationRequests != null)
                            organization.OrganizationManager.CreateOrganizationRequests.Clear();
                        if (organization.OrganizationManager.CreatePostRequests != null)
                            organization.OrganizationManager.CreatePostRequests.Clear();
                    }

                    var createOrganizationRequest =
                        _createOrganizationRequestRepository.GetCreateOrganizationRequestByOrganizationId(organization
                            .OrganizationID);
                    if (createOrganizationRequest != null)
                    {
                        createOrganizationRequest.Organization = null;
                        createOrganizationRequest.OrganizationManager = null;
                        createOrganizationRequest.Moderator = null;
                        organization.CreateOrganizationRequest = createOrganizationRequest;
                    }

                }
                return organizations.Where(o => o.IsDisable == false);
            }

        }

        public Organization? GetById(Guid id)
        {
            return _organizationRepository.GetById(id);
        }

        public Organization? CreateOrganization(CreateNewOrganization request)
        {
            var organizationManager = _organizationManagerRepository.GetById(request.OrganizationManagerID);
            if (organizationManager == null) { return null; }
            var organization = new Organization
            {
                OrganizationID = Guid.NewGuid(),
                OrganizationManagerID = request.OrganizationManagerID,
                Name = request.Name,
                Location = request.Location,
                Logo = request.Logo,
                Description = request.Description,
                Website = request.Website,
                Tax = request.Tax,
                FoundingDate = request.FoundingDate,
                OperatingLicense = request.OperatingLicense,
                Category = request.Category,
                Note = request.Note,
                CreatedAt = TimeHelper.GetTime(DateTime.UtcNow),
                IsActive = false,
                IsModify = false,
            };
            return _organizationRepository.Save(organization);
        }

        public async void UpdateOrganizationRequest(Guid organizationId, UpdateOrganizationRequest request)
        {
            var organization = _organizationRepository.GetById(organizationId);
            if (organization == null)
            {
                throw new NotFoundException("Không tìm thấy tổ chức này!");
            }

            var organizationRequest =
                _createOrganizationRequestRepository.GetCreateOrganizationRequestByOrganizationId(organizationId);
            if (organizationRequest != null)
            {
                if (organizationRequest.IsApproved) throw new BadRequestException("Tổ chức này hiện đã được duyệt, vì vậy mọi thông tin của tổ chức này không thể chỉnh sửa!");
            }
            if (!String.IsNullOrEmpty(request.Name))
            {
                organization.Name = request.Name;
            }
            if (request.Logo != null)
            {
                organization.Logo = await _firebaseService.UploadImage(request.Logo);
            }
            if (!String.IsNullOrEmpty(request.Description))
            {
                organization.Description = request.Description;
            }
            if (!String.IsNullOrEmpty(request.Website))
            {
                organization.Website = request.Website;
            }
            if (!String.IsNullOrEmpty(request.Tax))
            {
                organization.Tax = request.Tax;
            }
            if (!String.IsNullOrEmpty(request.Location))
            {
                organization.Location = request.Location;
            }
            if (!String.IsNullOrEmpty(request.FoundingDate.ToString()))
            {
                organization.FoundingDate = request.FoundingDate;
            }
            if (!String.IsNullOrEmpty(request.OperatingLicense))
            {
                organization.OperatingLicense = request.OperatingLicense;
            }
            if (!String.IsNullOrEmpty(request.Category))
            {
                organization.Category = request.Category;
            }
            if (!String.IsNullOrEmpty(request.Note))
            {
                organization.Note = request.Note;
            }
            _organizationRepository.Update(organization);
        }


        public void UpdateOrganizationStatusRequest(UpdateOrganizationStatusRequest request)
        {
            var organization = _organizationRepository.GetById(request.OrganizationId);
            if (organization == null)
            {
                throw new NotFoundException("Không tìm thấy tổ chức này!");
            }

            var organizationManagerRequest =
                _createOrganizationRequestRepository.GetCreateOrganizationRequestByOrganizationId(
                    request.OrganizationId);
            if (organizationManagerRequest != null)
            {
                if(organizationManagerRequest.IsApproved)
                    throw new BadRequestException(
                        "Tổ chức này hiện đã được duyệt, vì vậy mọi thông tin của tổ chức này không thể chỉnh sửa!");
            }

            if (request.IsDisable)
            {
                organization.IsActive = false;
                organization.IsDisable = true;
            }
            organization.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
            _organizationRepository.Update(organization);
        }
    }
}
