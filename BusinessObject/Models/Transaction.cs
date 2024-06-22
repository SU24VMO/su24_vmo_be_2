using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TransactionStatus = BusinessObject.Enums.TransactionStatus;

namespace BusinessObject.Models
{
    public class Transaction
    {
        public Guid TransactionID { get; set; } = default!;
        public Guid CampaignID { get; set; } = default!;
        public TransactionType TransactionType { get; set; } = default!;
        public float Amount { get; set; } = default!;
        public string Note { get; set; } = default!;
        public string PayerName { get; set; } = default!;
        public bool IsIncognito { get; set; } = default!;
        public DateTime CreateDate { get; set; } = default!;
        public int OrderId { get; set; } = default!;
        public Guid AccountId { get; set; }
        public Guid? BankingAccountID { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public string TransactionQRImageUrl { get; set; } = default!;
        public BankingAccount? BankingAccount { get; set; }
        public Account? Account { get; set; }
        public Campaign? Campaign { get; set; }
    }
}
