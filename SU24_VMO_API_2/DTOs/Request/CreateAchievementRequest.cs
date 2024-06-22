using BusinessObject.Models;
using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateAchievementRequest
    {
        [Required]
        public Guid OrganizationID { get; set; } = default!;
        [Required]
        public string Title { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        public string? Link { get; set; }
    }
}
