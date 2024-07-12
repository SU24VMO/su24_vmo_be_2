namespace SU24_VMO_API.DTOs.Request
{
    public class CreateActivityRequestRequest
    {
        public Guid ProcessingPhaseId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public Guid AccountId { get; set; } = default!;
        public List<IFormFile>? ActivityImages { get; set; } = default!;

    }
}
