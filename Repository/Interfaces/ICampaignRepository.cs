using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICampaignRepository : ICrudBaseRepository<Campaign, Guid>
    {
        public IEnumerable<Campaign> GetCampaignsByCampaignName(string campaignName);
        public IEnumerable<Campaign> GetCampaignsByCampaignTypeId(Guid campaignTypeId);

    }
}
