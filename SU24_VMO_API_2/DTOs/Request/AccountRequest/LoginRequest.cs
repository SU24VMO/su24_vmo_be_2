using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class LoginRequest
    {
        public string? Account { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
