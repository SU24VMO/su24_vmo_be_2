using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICreateOrganizationRequestRepository : ICrudBaseRepository<CreateOrganizationRequest, Guid>
    {
        public CreateOrganizationRequest GetCreateOrganizationRequestByOrganizationId(Guid organizationId);
        public IEnumerable<CreateOrganizationRequest> GetOrganizationRequestsByOrganizationName(string? organizationName, int? pageSize, int? pageNo);
        public IEnumerable<CreateOrganizationRequest> GetOrganizationRequestsByOrganizationName(string? organizationName);


    }
}
