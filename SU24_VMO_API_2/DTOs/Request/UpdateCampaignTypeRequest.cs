namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateCampaignTypeRequest
    {
        public Guid CampaignTypeID { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public bool? IsValid { get; set; } = default!;
    }
}
