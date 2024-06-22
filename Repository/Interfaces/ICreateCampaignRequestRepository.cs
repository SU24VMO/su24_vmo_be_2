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
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByOrganizationManagerId(Guid organizationManagerId);
        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByMemberId(Guid userId);
        public CreateCampaignRequest? GetCreateCampaignRequestByCampaignId(Guid campaignId);

    }
}
