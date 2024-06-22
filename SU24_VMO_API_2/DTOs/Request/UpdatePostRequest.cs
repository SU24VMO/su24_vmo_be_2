namespace SU24_VMO_API.DTOs.Request
{
    public class UpdatePostRequest
    {
        public Guid PostId { get; set; }
        public IFormFile? Cover { get; set; } = default!;
        public string? Title { get; set; } = default!;
        public string? Content { get; set; } = default!;
        public IFormFile? Image { get; set; } = default!;
    }
}
