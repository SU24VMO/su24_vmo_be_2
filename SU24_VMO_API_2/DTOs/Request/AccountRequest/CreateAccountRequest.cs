using BusinessObject.Enums;
using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class CreateAccountRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
        [Required]
        public string Username { get; set; } = default!;
        public string? Avatar { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
