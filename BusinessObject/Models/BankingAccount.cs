using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class BankingAccount
    {
        public Guid BankingAccountID { get; set; } = default!;
        public string BankingName { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string QRCode { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid AccountId { get; set; } = default!;
        public bool IsAvailable { get; set; } = default!;
        public virtual Account? Account { get; set; }
        public virtual List<Transaction>? Transactions { get; set; }
        public virtual List<Campaign>? Campaigns { get; set; }
    }
}
