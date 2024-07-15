using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class ActivityResponse
    {
        public Guid ActivityId { get; set; } = default!;
        public Guid ProcessingPhaseId { get; set; } = default!;
        public string? OrganizationName { get; set; } = default!;
        public string? CampaignName { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; } = default!;

        public ProcessingPhase? ProcessingPhase { get; set; }
        public CreateActivityRequest? CreateActivityRequest { get; set; }
        public List<ActivityImage>? ActivityImages { get; set; }
    }
}
