using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class CreateCampaignTypeRequest
    {
        [Required]
        public string Name { get; set; } = default!;
    }
}
