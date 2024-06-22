namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class UpdateCreateCampaignRequest
    {
        public Guid? CreateCampaignRequestID { get; set; } = default!;
        public bool? IsApproved { get; set; } = default!;
    }
}
