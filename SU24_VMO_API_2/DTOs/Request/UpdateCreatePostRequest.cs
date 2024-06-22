using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateCreatePostRequest
    {
        //public IFormFile? Cover { get; set; } = default!;
        //public string? Title { get; set; } = default!;
        //public string? Content { get; set; } = default!;
        //public IFormFile? Image { get; set; } = default!;
        //[Required]
        //public Guid CreatePostRequestId { get; set; }
        //[Required]
        //public Guid RequestManagerId { get; set; }
        [Required]
        public Guid CreatePostRequestId { get; set; }
        [Required]
        public Guid RequestManagerId { get; set; }
        [Required]
        public bool IsAccept {  get; set; }

    }
}
