using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateActivityImageRequest
    {
        [Required]
        public Guid ActivityId { get; set; } = default!;
        public List<IFormFile?> Images { get; set; } = default!;
    }
}
