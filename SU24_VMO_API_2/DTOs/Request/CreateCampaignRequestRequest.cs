using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateCampaignRequestRequest
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
        public Guid? OrganizationId { get; set; } = default!;
        //[Required]
        //public Guid CampaignId { get; set; } = default!;
        public IFormFile? ApplicationConfirmForm { get; set; } = default!;
        public IFormFile? ImageCampaign { get; set; } = default!;
        [Required]
        public string BankingName { get; set; } = default!;
        [Required]
        public string AccountName { get; set; } = default!;
        [Required]
        public string BankingAccountNumber { get; set; } = default!;
        [Required]
        public IFormFile QRCode { get; set; } = default!;
    }
}
