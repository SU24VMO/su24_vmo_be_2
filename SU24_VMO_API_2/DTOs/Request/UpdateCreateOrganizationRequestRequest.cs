using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateCreateOrganizationRequestRequest
    {
        public string? OrganizationName { get; set; } = default!;
        public IFormFile? Logo { get; set; } = default!;
        public string? OrganizationManagerEmail { get; set; } = default!;
        public string? OrganizationTaxCode { get; set; } = default!;
        public DateTime? FoundingDate { get; set; } = default!;
        public string? SocialMediaLink { get; set; } = default!;
        public string? AreaOfActivity { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? PlanInformation { get; set; } = default!;
        public string? AchievementLink { get; set; } = default!;
        public string? AuthorizationDocuments { get; set; } = default!;
    }
}
