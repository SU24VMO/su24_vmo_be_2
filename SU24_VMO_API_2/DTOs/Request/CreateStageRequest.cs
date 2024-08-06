using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API_2.DTOs.Request
{
    public class CreateStageRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Amount { get; set; }
    }
}
