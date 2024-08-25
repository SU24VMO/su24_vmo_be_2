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
        public IEnumerable<Campaign> GetCampaignsCreateByOM();
        public IEnumerable<Campaign> GetCampaignsCreateByVolunteer();
        public IEnumerable<Campaign> GetAll(int pageNumber, int pageSize, string? status, Guid? campaignTypeId, string? createBy, string? campaignName);
    }
}
