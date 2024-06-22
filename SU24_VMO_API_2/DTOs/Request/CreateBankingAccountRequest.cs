namespace SU24_VMO_API.DTOs.Request
{
    public class CreateBankingAccountRequest
    {
        public string BankingName { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string QRCode { get; set; } = default!;
        public Guid AccountId { get; set; } = default!;
    }
}
