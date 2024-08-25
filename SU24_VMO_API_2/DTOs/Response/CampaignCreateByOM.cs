using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CampaignCreateByOm
    {
        public List<Campaign> Campaigns { get; set; }
        public int TotalItem { get; set; }
    }
}
