using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICreateOrganizationManagerRequestRepository : ICrudBaseRepository<CreateOrganizationManagerRequest, Guid>
    {
        public IEnumerable<CreateOrganizationManagerRequest> GetCreateOrganizationManagerRequestsWithEmail(string email);
        public IEnumerable<CreateOrganizationManagerRequest> GetCreateOrganizationManagerRequestsWithPhoneNumber(string phoneNumber);
    }
}
