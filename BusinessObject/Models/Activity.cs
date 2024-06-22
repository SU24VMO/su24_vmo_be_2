using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Activity
    {
        public Guid ActivityId { get; set; } = default!;
        public Guid ProcessingPhaseId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; } = default!;

        public ProcessingPhase? ProcessingPhase { get; set; }
        public List<ActivityImage>? ActivityImages { get; set; }
    }
}
