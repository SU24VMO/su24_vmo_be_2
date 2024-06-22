using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class CreateOrganizationRequest
    {
        public Guid CreateOrganizationRequestID { get; set; } = default!;
        public Guid OrganizationID { get; set;} = default!;
        public string? OrganizationName { get; set; } = default!;
        public string? OrganizationManagerEmail { get; set; } = default!;
        public string? OrganizationTaxCode { get; set; } = default!;
        public DateTime? FoundingDate { get; set; } = default!;
        public string? SocialMediaLink { get; set; } = default!;
        public string? AreaOfActivity { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? PlanInformation { get; set; } = default!;
        public string? AchievementLink { get; set; } = default!;
        public string? AuthorizationDocuments { get; set; } = default!;

        public Guid CreateBy { get; set; } = default!;
        public Guid? ApprovedBy { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Guid? ModifiedBy { get; set; } = default!;


        public bool IsApproved { get; set; } = default!;
        public bool IsRejected { get; set; } = default!;
        public bool IsPending { get; set; } = default!;
        public bool IsLocked { get; set; } = default!;

        public Organization? Organization { get; set; }
        public OrganizationManager? OrganizationManager { get; set; }
        public RequestManager? RequestManager { get; set; }
    }
}
