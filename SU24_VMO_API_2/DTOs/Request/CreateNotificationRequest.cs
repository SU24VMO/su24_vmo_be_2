using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateNotificationRequest
    {
        [Required]
        public Guid AccountID { get; set; }

    }
}
