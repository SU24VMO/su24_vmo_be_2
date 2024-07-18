using BusinessObject.Enums;
using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class TransactionWithCampaignNameResponse
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
        public string TransactionImageUrl { get; set; } = default!;
        public string? CampaignName { get; set; } = default!;
        public BankingAccount? BankingAccount { get; set; }
        public Account? Account { get; set; }
        public Campaign? Campaign { get; set; }
    }
}
