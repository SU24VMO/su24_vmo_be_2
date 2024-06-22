using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateNewOrganizationManagerRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
        [Required]
        public string Username { get; set; } = default!;
        public string? Avatar { get; set; }
        public string PhoneNumber { get; set; } = default!;
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [Required]
        public string Gender { get; set; } = default!;
        public DateTime BirthDay { get; set; } = default!;
        public string? FacebookUrl { get; set; } = default!;
        public string? YoutubeUrl { get; set; } = default!;
        public string? TiktokUrl { get; set; } = default!;
    }
}
