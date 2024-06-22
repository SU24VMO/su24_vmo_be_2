using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateTransactionRequest
    {
        [Required]
        public string Note { get; set; } = default!;
        [Required]
        public int Price { get; set; } = default!;
        [Required]
        public bool IsIncognito { get; set; } = default!;
        [Required]
        public Guid AccountId { get; set; }
        [Required]
        public Guid CampaignId { get; set; }

    }
}
