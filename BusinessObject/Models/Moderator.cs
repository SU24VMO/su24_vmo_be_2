using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Moderator
    {
        public Guid ModeratorID { get; set; } = default!;
        public Guid AccountID { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public virtual Account? Account { get; set; }
        public virtual List<CreateVolunteerRequest>? CreateVolunteerRequests { get; set; }
        public virtual List<CreatePostRequest>? CreatePostRequests { get; set; }
        public virtual List<CreateOrganizationRequest>? CreateOrganizationRequests { get; set; }
        public virtual List<CreateOrganizationManagerRequest>? CreateOrganizationManagerRequests { get; set; }
        public virtual List<CreateCampaignRequest>? CreateCampaignRequests { get; set; }
        public virtual List<CreateActivityRequest>? CreateActivityRequests { get; set; }

    }
}
