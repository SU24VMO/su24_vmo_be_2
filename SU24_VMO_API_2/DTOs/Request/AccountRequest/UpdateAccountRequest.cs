using BusinessObject.Enums;
using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class UpdateAccountRequest
    {
        public Guid AccountID { get; set; }
        public string? Username { get; set; } = default!;
        public string? Avatar { get; set; }
        public string? Email { get; set; } = default!;
        public string? FirstName { get; set; } = default!;
        public string? LastName { get; set; } = default!;

        public string? PhoneNumber { get; set; } = default!;

        public string? Gender { get; set; } = default!;
        public DateTime? BirthDay { get; set; } = default!;

        public string? FacebookUrl { get; set; } = default!;
        public string? YoutubeUrl { get; set; } = default!;
        public string? TiktokUrl { get; set; } = default!;
        public bool IsActived { get; set; } = default!;
        public bool IsBlocked { get; set; } = default!;
    }
}
