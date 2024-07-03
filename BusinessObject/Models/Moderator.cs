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
        public Account? Account { get; set; }
        public List<CreateVolunteerRequest>? CreateVolunteerRequests { get; set; }
        public List<CreatePostRequest>? CreatePostRequests { get; set; }
        public List<CreateOrganizationRequest>? CreateOrganizationRequests { get; set; }
        public List<CreateOrganizationManagerRequest>? CreateOrganizationManagerRequests { get; set; }
        public List<CreateCampaignRequest>? CreateCampaignRequests { get; set; }
        public List<CreateActivityRequest>? CreateActivityRequests { get; set; }

    }
}
