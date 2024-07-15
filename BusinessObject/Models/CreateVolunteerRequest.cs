using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class CreateVolunteerRequest
    {
        public Guid CreateVolunteerRequestID { get; set; } = default!;
        public Guid MemberID { get; set; } = default!;
        public string MemberName { get; set; } = default!;
        public DateTime Birthday { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? SocialMediaLink { get; set; } = default!;
        public string? CitizenIdentification { get; set; } = default!;
        public string MemberAddress { get; set; } = default!;
        public RoleInClub? RoleInClub { get; set; } = default!;
        public string? ClubName { get; set; } = default!;
        public string? DetailDescriptionLink { get; set; } = default!;
        public string? AchievementLink { get; set; } = default!;
        public Guid CreateBy { get; set; } = default!;
        public Guid? ApprovedBy { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? ModifiedBy { get; set; } = default!;

        public bool IsAcceptTermOfUse { get; set; } = default!;
        public bool IsApproved { get; set; } = default!;
        public bool IsRejected { get; set; } = default!;
        public bool IsPending { get; set; } = default!;
        public bool IsLocked { get; set; } = default!;

        public Member? Member { get; set; }
        public Moderator? Moderator { get; set; }
    }
}
