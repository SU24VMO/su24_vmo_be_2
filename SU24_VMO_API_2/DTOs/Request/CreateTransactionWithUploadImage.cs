namespace SU24_VMO_API_2.DTOs.Request
{
    public class CreateTransactionWithUploadImage
    {
        public Guid AccountId { get; set; } = default!;
        public Guid CampaignId { get; set; } = default!;
        public Guid BankingAccountId { get; set; } = default!;
        public float Amount { get; set; } = default!;
        public IFormFile TransactionImage { get; set; } = default!;
    }
}
