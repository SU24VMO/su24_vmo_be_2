﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ActivityStatementFile
    {
        public Guid ActivityStatementFileId { get; set; } = default!;
        public Guid ActivityId { get; set; } = default!;
        public string? Description { get; set; }
        public string Link { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; } = default!;
        public virtual Activity? Activity { get; set; }
    }
}