using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class CreateActivityRequest
    {
        public Guid CreateActivityRequestID { get; set; } = default!;
        public Guid ActivityID { get; set; } = default!;
        public Guid? CreateByOM { get; set; } = default!;
        public Guid? CreateByMember { get; set; } = default!;
        public Guid? ApprovedBy { get; set; } = default!;
        public Guid? ModifiedBy { get; set; } = default!;

        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public bool IsApproved { get; set; } = default!;
        public bool IsRejected { get; set; } = default!;
        public bool IsPending { get; set; } = default!;
        public bool IsLocked { get; set; } = default!;

        public OrganizationManager? OrganizationManager { get; set; }
        public Member? Member { get; set; }
        public Activity? Activity { get; set; }
        public Moderator? Moderator { get; set; }
    }
}
