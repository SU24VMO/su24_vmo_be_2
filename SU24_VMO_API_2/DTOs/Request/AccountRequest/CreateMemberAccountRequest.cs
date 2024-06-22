using BusinessObject.Enums;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class CreateMemberAccountRequest
    {
        public Guid UserID { get; set; } = default!;
        public string MemberName { get; set; } = default!;
        public DateTime Birthday { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? SocialMediaLink { get; set; } = default!;
        public string MemberAddress { get; set; } = default!;
        public RoleInClub? RoleInClub { get; set; } = default!;
        public string? ClubName { get; set; } = default! ;
        public string? DetailDescriptionLink { get; set; } = default!;
        public string? AchievementLink { get; set; } = default!;
        public bool IsAcceptTermOfUse { get; set; } = default!;
    }
}
