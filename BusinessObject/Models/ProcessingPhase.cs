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
        public int Priority { get; set; }
        public string CurrentMoney { get; set; } = default!;
        public double Percent { get; set; }
        public double CurrentPercent { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsEnd { get; set; } = default!;
        public Guid? UpdateBy { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Campaign? Campaign { get; set; }

        public List<Activity>? Activities { get; set; }
        public List<ProcessingPhaseStatementFile> ProcessingPhaseStatementFiles { get; set; }

    }
}
