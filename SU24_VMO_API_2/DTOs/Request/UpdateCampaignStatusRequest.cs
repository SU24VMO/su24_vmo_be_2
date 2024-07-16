namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateCampaignStatusRequest
    {
        public Guid CampaignId { get; set; } = default!;
        public bool IsDisable { get; set; } = default!;
    }
}
