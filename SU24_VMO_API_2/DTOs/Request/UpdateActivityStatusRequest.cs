using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateActivityStatusRequest
    {
        [Required]
        public Guid ActivityId { get; set; } = default!;
        public bool IsActive { get; set; } = default!;
    }
}
