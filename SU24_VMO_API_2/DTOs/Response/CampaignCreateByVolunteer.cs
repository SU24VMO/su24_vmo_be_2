using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CampaignCreateByVolunteer
    {
        public List<CampaignResponse> Campaigns { get; set; }
        public int TotalItem { get; set; }
    }
}
