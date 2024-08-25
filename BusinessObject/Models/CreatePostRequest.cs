using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class CreatePostRequest
    {
        public Guid CreatePostRequestID { get; set; } = default!;
        public Guid PostID { get; set; } = default!;
        public Guid? CreateByMember { get; set; } = default!;
        public Guid? CreateByOM { get; set; } = default!;

        public Guid? ApprovedBy { get; set; } = default!;
        public DateTime? ApprovedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? ModifiedBy { get; set; } = default!;

        public bool IsApproved { get; set; } = default!;
        public bool IsRejected { get; set; } = default!;
        public bool IsPending { get; set; } = default!;
        public bool IsLocked { get; set; } = default!;

        public virtual Post? Post { get; set; }
        public virtual OrganizationManager? OrganizationManager { get; set; }
        public virtual Moderator? Moderator { get; set; }
        public virtual Member? Member { get; set; }
    }
}
