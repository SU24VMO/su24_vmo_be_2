namespace SU24_VMO_API_2.DTOs.Response
{
    public class CampaignTierIIWithBankingAccountResponse
    {
        public Guid CampaignID { get; set; } = default!;
        public Guid? BankingAccountId { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string? QRCode { get; set; } = default!;
        public string? AccountName { get; set; } = default!;
        public string? BankingName { get; set; } = default!;
        public string? Amount { get; set; } = default!;
        public string ProcessingPhaseName { get; set; } = default!;
        public bool IsEligible { get; set; } = default!;
        public string CurrentMoney { get; set; } = default!;
        public double ProcessingPhasePercent { get; set; } = default!;
        public double Percent { get; set; } = default!;
        public string? TransactionImage { get; set; } = default!;
        public bool DonatePhaseIsEnd { get; set; } = default!;
        public string? BankingAccountNumber { get; set; } = default!;
        public bool IsActive { get; set; } = default!;
        public bool IsDisable { get; set; } = default!;
        public bool IsComplete { get; set; } = default!;
        public bool IsProcessing { get; set; } = default!;
        public Guid? ProcessingPhaseId { get; set; } = default!;
    }
}
