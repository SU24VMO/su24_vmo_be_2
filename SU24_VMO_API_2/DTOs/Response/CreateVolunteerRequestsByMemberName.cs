using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CreateVolunteerRequestsByMemberName
    {
        public List<CreateVolunteerRequest> CreateVolunteerRequests { get; set; }
        public int TotalITem { get; set; }
    }
}
