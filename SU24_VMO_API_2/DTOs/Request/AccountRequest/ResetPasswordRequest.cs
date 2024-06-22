using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
    }
}
