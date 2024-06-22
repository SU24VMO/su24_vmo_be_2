using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class DonatePhase
    {
        public Guid DonatePhaseId { get; set; } = default!;
        public Guid CampaignId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CurrentMoney { get; set; } = default!;
        public double Percent { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsEnd { get; set; } = default!;
        public DateTime? UpdateDate { get; set; }
        public Campaign? Campaign { get; set; }
    }
}
