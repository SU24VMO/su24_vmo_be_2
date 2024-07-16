using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateOrganizationStatusRequest
    {
        [Required]
        public Guid OrganizationId { get; set; } = default!;
        public bool IsDisable { get; set; } = default;
    }
}
