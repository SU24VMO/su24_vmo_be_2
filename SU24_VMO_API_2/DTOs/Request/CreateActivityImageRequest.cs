namespace SU24_VMO_API.DTOs.Request
{
    public class CreateActivityImageRequest
    {
        public Guid ActivityId { get; set; } = default!;
        public List<IFormFile> Images { get; set; } = default!;
    }
}
