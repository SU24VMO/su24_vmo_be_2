using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateCreatePostRequest
    {
        [Required]
        public Guid CreatePostRequestId { get; set; }
        [Required]
        public Guid RequestManagerId { get; set; }
        [Required]
        public bool IsApproved {  get; set; }

    }
}
