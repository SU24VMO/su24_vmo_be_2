using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ProcessingPhaseStatementFile
    {
        public Guid ProcessingPhaseStatementFileId { get; set; } = default!;
        public Guid ProcessingPhaseId { get; set; } = default!;
        public string Link { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid CreateBy { get; set; }
        public ProcessingPhase? ProcessingPhase { get; set; }
    }
}
