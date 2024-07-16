using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdatePostStatusRequest
    {
        [Required]
        public Guid PostId { get; set; } = default!;
        public bool IsDisable { get; set; } = default!;
    }
}
