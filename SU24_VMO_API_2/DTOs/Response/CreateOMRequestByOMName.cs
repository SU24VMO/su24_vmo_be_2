using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CreateOMRequestByOMName
    {
        public List<CreateOrganizationManagerRequest> CreateOrganizationManagerRequests { get; set; }
        public int TotalItem { get; set; }
    }
}
