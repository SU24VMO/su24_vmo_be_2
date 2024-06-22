using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateAchievementRequest
    {
        [Required]
        public Guid AchievementID { get; set; } 
        public string? Title { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public string? Link { get; set; }
    }
}
