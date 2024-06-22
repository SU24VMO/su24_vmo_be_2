namespace SU24_VMO_API.DTOs.Request
{
    public class CreateActivityImageRequest
    {
        public Guid ActivityId { get; set; } = default!;
        public string Link { get; set; } = default!;
    }
}
