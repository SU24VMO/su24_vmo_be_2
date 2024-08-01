using BusinessObject.Enums;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class TransactionForStatementByAdmin
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string? SendAccount { get; set; }
        public string? ReceiveAccount { get; set; }
        public string? CampaignName { get; set; }
        public string Amount { get; set; }
        public string? Platform { get; set;}
        public TransactionStatus Status { get; set; }
        public bool? IsCognito { get; set; }
        public string? TransactionImageUrl { get; set; }
    }
}
