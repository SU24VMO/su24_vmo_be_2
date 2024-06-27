using BusinessObject.Models;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X509;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class OrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;

        public OrganizationService(IOrganizationRepository organizationRepository, IOrganizationManagerRepository organizationManagerRepository)
        {
            _organizationRepository = organizationRepository;
            _organizationManagerRepository = organizationManagerRepository;
        }

        public IEnumerable<Organization> GetAllOrganizations()
        {
            var organizations = _organizationRepository.GetAll();
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
            }
            return organizations;
        }

        public IEnumerable<Organization> GetAllOrganizationsByOrganizationName(string organizationName)
        {
            var organizations = _organizationRepository.GetOrganizationsByOrganizationName(organizationName);
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
            }
            return organizations;
        }
        public IEnumerable<Organization> GetAllOrganizationsByOrganizationManagerId(Guid organizationManagerId)
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


            }
            return organizations;
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

        public void UpdateOrganizationRequest(UpdateOrganizationRequest request)
        {
            var organization = _organizationRepository.GetById(request.OrganizationID);
            if (organization == null) { return; }
            if (!String.IsNullOrEmpty(request.Name))
            {
                organization.Name = request.Name;
            }
            if (!String.IsNullOrEmpty(request.Logo))
            {
                organization.Logo = request.Logo;
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
    }
}
