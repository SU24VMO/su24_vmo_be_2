namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateCampaignRequest
    {
        public Guid? OrganizationID { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public IFormFile? Image { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime? ExpectedEndDate { get; set; }
        public string? ApplicationConfirmForm { get; set; } = default!;
        public bool? IsTransparent { get; set; } = default!;
        public bool? IsActive { get; set; } = default!;
        public bool? IsModify { get; set; } = default!;
        public bool? IsComplete { get; set; } = default!;
        public bool? CanBeDonated { get; set; } = default!;
        public string? Note { get; set; } = default!;

    }
}
