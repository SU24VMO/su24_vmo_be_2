using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API_2.DTOs.Request.AccountRequest
{
    public class CheckPasswordRequest
    {
        [Required]
        public string Email { get; set; } = default!;
        [Required]
        public string OldPassword { get; set; } = default!;
    }
}
