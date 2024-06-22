namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateCreateOrganizationRequest
    {
        public Guid CreateOrganizationRequestID { get; set; } = default!;
        public string? OrganizationName { get; set; } = default!;
        public string? OrganizationManagerEmail { get; set; } = default!;
        public string? OrganizationTaxCode { get; set; } = default!;
        public DateTime? FoundingDate { get; set; } = default!;
        public string? SocialMediaLink { get; set; } = default!;
        public string? AreaOfActivity { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? PlanInformation { get; set; } = default!;
        public string? AchievementLink { get; set; } = default!;
        public string? AuthorizationDocuments { get; set; } = default!;
        public DateTime? UpdateDate { get; set; }
    }
}
