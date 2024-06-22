using BusinessObject.Models;
using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateNewCampaignRequest
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Address { get; set; } = default!;
        [Required]
        public Guid CampaignTypeId { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public DateTime StartDate { get; set; } = default!;
        [Required]
        public DateTime ExpectedEndDate { get; set; } = default!;
        [Required]
        public string TargetAmount { get; set; } = default!;
        public Guid? OrganizationID { get; set; } = default!;
        public string? ApplicationConfirmForm { get; set; } = default!;
        public IFormFile ImageCampaign {  get; set; } = default!;
        public string BankingName { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string BankingAccountNumber { get; set; } = default!;
        public IFormFile QRCode { get; set; } = default!;
    }
}
