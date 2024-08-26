using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CreateOrganizationRequestByOrganizationName
    {
        public List<CreateOrganizationRequest> CreateOrganizationRequests { get; set; }
        public int TotalItem { get; set; }
    }
}
