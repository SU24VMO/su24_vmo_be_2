namespace SU24_VMO_API.DTOs.Request
{
    public class CreateNewActivityRequest
    {
        public Guid ProcessingPhaseId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
    }
}
