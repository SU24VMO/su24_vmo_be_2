using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class CreateOrganizationManagerRequest
    {
        public Guid CreateOrganizationManagerRequestID { get; set; } = default!;
        public Guid OrganizationManagerID { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string? PhoneNumber { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? CitizenIdentification { get; set; } = default!;
        public string? PersonalTaxCode { get; set; } = default!;
        public Guid CreateBy { get; set; } = default!;
        public Guid? ApprovedBy { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Guid? ModifiedBy { get; set; } = default!;

        public bool IsAcceptTermOfUse { get; set; } = default!;
        public bool IsApproved { get; set; } = default!;
        public bool IsRejected { get; set; } = default!;
        public bool IsPending { get; set; } = default!;
        public bool IsLocked { get; set; } = default!;

        public OrganizationManager? OrganizationManager { get; set; }
        public RequestManager? RequestManager { get; set; }
    }
}
