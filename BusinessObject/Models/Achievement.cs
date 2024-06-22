using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Achievement
    {
        public Guid AchievementID { get; set; } = default!;
        public Guid OrganizationID { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? Link { get; set;}
        public DateTime? CreatedDate { get; set; } = default!;
        public Organization? Organization { get; set; }
    }
}
