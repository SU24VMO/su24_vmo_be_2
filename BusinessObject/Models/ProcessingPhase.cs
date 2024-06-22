using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ProcessingPhase
    {
        public Guid ProcessingPhaseId { get; set; } = default!;
        public Guid CampaignId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsEnd { get; set; } = default!;
        public DateTime? UpdateDate { get; set; }
        public Campaign? Campaign { get; set; }

        public List<Activity>? Activities { get; set; }

    }
}
