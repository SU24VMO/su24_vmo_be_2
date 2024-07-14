namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateCampaignStatusRequest
    {
        public string? CampaignId { get; set; } = default!;
        public bool IsEnd { get; set; } = default!;
    }
}
