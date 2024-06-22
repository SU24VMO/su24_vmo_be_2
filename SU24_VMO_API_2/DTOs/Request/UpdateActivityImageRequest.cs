namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateActivityImageRequest
    {
        public Guid ActivityImageId { get; set; } = default!;
        public string? Link { get; set; } = default!;
        public bool? IsActive { get; set; } = default!;
    }
}
