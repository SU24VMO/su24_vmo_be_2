using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class User
    {
        public Guid UserID { get; set; } = default!;
        public Guid AccountID { get; set; } = default!;
        public string? PhoneNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Gender { get; set; } = default!;
        public DateTime? BirthDay { get; set; } = default!;
        public string? FacebookUrl { get; set; } = default!;
        public string? YoutubeUrl { get; set; } = default!;
        public string? TiktokUrl { get; set; } = default!;
        public bool IsVerified { get; set; } = default!;
        public Account? Account { get; set; }
        public List<CreateMemberRequest>? CreateUserVerifiedRequests { get; set; }
        public List<CreateCampaignRequest>? CreateCampaignRequests { get; set; }
        public List<BankingAccount>? BankingAccounts { get; set; }
        public List<CreatePostRequest>? CreatePostRequests { get; set; }

    }
}

