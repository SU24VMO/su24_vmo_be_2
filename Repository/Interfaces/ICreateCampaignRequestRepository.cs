using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICreateCampaignRequestRepository : ICrudBaseRepository<CreateCampaignRequest, Guid>
    {
        public CreateCampaignRequest? SaveWithBankingAccount(CreateCampaignRequest entity, BankingAccount bankingAccount);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByOrganizationManagerId(Guid organizationManagerId, int? pageSize, int? pageNo);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByOrganizationManagerId(Guid organizationManagerId);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByOrganizationManagerId(Guid organizationManagerId, string? status);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByVolunteerId(Guid memberId, int? pageSize, int? pageNo);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByVolunteerId(Guid memberId);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignTierIRequestByVolunteerId(Guid memberId, int? pageSize, int? pageNo);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignTierIRequestByVolunteerId(Guid memberId);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByVolunteerId(Guid memberId, string? status);
        public CreateCampaignRequest? GetCreateCampaignRequestByCampaignId(Guid campaignId);
        public Task<CreateCampaignRequest?> SaveWithBankingAccountAsync(CreateCampaignRequest entity, BankingAccount bankingAccount);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestsByCampaignName(string? name, int? pageSize, int? pageNo);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestsByCampaignName(string? name);
    }
}
