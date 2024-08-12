using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateOrganizationRequestRequest
    {
        [Required]
        public string OrganizationName { get; set; } = default!;
        [Required]
        public IFormFile Logo { get; set; } = default!;
        [Required]
        public string OrganizationManagerEmail { get; set; } = default!;
        [Required]
        public string OrganizationTaxCode { get; set; } = default!;
        [Required]
        public DateTime FoundingDate { get; set; } = default!;
        public string? SocialMediaLink { get; set; } = default!;
        public string? AreaOfActivity { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? PlanInformation { get; set; } = default!;
        public string? AchievementLink { get; set; } = default!;
        public IFormFile? AuthorizationDocuments { get; set; } = default!;
    }
}
