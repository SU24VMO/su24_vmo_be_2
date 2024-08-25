using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CampaignCreateByVolunteerResponse
    {
        public List<Campaign> Campaigns { get; set; }
        public int TotalItem { get; set; }
    }
}
