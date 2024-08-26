using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CreateCampaignRequestByName
    {
        public List<CreateCampaignRequest> CreateCampaignRequests { get; set; }
        public int TotalItem { get; set; }
    }
}
