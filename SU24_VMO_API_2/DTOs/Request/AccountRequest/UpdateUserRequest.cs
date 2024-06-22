using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class UpdateUserRequest
    {
        public string? Email { get; set; } = default!;
        public string? FirstName { get; set; } = default!;
        public string? LastName { get; set; } = default!;
        public string? Username { get; set; } = default!;

        public string? PhoneNumber { get; set; } = default!;

        public string? Gender { get; set; } = default!;
        public DateTime? BirthDay { get; set; } = default!;

        public string? FacebookUrl { get; set; } = default!;
        public string? YoutubeUrl { get; set; } = default!;
        public string? TiktokUrl { get; set; } = default!;
    }
}
