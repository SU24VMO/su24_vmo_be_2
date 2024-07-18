namespace SU24_VMO_API_2.DTOs.Response
{
    public class CampaignWithBankingAccountResponse
    {
        public Guid CampaignID { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string? QRCode { get; set; } = default!;
        public string? AccountName { get; set; } = default!;
        public string? BankingName { get; set; } = default!;

    }
}
