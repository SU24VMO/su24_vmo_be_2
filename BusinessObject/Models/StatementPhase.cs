﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class StatementPhase
    {
        public Guid StatementPhaseId { get; set; } = default!;
        public Guid CampaignId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsEnd { get; set; } = default!;
        public Guid? UpdateBy { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? UpdateDate { get; set; }
        public virtual Campaign? Campaign { get; set; }
        public virtual List<StatementFile>? StatementFiles { get; set; }
    }
}
