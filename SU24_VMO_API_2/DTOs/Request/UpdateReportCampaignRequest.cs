namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateReportCampaignRequest
    {
        public Guid CampaignId { get; set; } = default!;
        public bool IsTransparent { get; set; } = default!;
    }
}
