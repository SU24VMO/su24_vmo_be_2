using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreatePostRequestRequest
    {
        [Required]
        public IFormFile Cover { get; set; } = default!;
        [Required]
        public string Title { get; set; } = default!;
        [Required]
        public string Content { get; set; } = default!;
        [Required]
        public IFormFile Image { get; set; } = default!;
        public string? Description { get; set; } = default!;
        [Required]
        public Guid AccountId { get; set; }
    }
}
