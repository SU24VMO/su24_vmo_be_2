﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class CreateCampaignRequest
    {
        public Guid CreateCampaignRequestID { get; set; } = default!;
        public Guid CampaignID { get; set;} = default!;
        public Guid? CreateByMember { get; set; } = default!;
        public Guid? CreateByOM { get; set; } = default!;

        public Guid? ApprovedBy { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Guid? ModifiedBy { get; set; } = default!;

        public bool IsApproved { get; set; } = default!;
        public bool IsRejected { get; set; } = default!;
        public bool IsPending { get; set; } = default!;
        public bool IsLocked { get; set; } = default!;

        public virtual Campaign? Campaign { get; set; }
        public virtual Member? Member { get; set; }
        public virtual OrganizationManager? OrganizationManager { get; set; }
        public virtual Moderator? Moderator { get; set; }

    }
}
