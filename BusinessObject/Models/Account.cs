using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Account
    {
        public Guid AccountID { get; set; }
        [JsonIgnore]
        public byte[] HashPassword { get; set; } = default!;
        [JsonIgnore]
        public byte[] SaltPassword { get; set; } = default!;
        [EmailAddress]
        public string Email { get; set; } = default!;
        public string? Username { get; set; } = default!;
        public string? Avatar { get; set; }
        public Role Role { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActived { get; set; } = default!;
        public bool IsBlocked { get; set; } = default!;
        public Guid? ModifiedBy { get; set; } = default!;
        public virtual List<Notification>?  Notifications { get; set; }
        public virtual List<AccountToken>?  AccountTokens { get; set; }
        public virtual List<BankingAccount>? BankingAccounts { get; set; }
        public virtual List<Transaction>? Transactions { get; set; }
        public virtual List<IPAddress>? IPAddresses { get; set; }
    }
}
