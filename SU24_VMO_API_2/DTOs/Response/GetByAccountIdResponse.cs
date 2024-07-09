using BusinessObject.Enums;
using BusinessObject.Models;
using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class GetByAccountIdResponse
    {
        public Guid AccountID { get; set; }
        public byte[] HashPassword { get; set; } = default!;
        public byte[] SaltPassword { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Username { get; set; } = default!;
        public string? Avatar { get; set; }
        public Role Role { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActived { get; set; } = default!;
        public bool IsBlocked { get; set; } = default!;
        public Guid? ModifiedBy { get; set; } = default!;
        public float? DonatedMoney { get; set; } = default!;
        public int? NumberOfDonations { get; set; } = default!;
        public string? FirstName { get; set; } = default!;
        public string? LastName { get; set; } = default!;
        public string? LinkFacebook { get; set; } = default!;
        public string? LinkYoutube { get; set; } = default!;
        public string? LinkTiktok { get; set; } = default!;
        public List<Notification>? Notifications { get; set; }
        public List<AccountToken>? AccountTokens { get; set; }
        public List<BankingAccount>? BankingAccounts { get; set; }
        public List<Transaction>? Transactions { get; set; }
    }
}
