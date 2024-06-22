using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IOrganizationRepository : ICrudBaseRepository<Organization, Guid>
    {
        public IEnumerable<Organization> GetOrganizationsByOrganizationName(string organizationName);
        public IEnumerable<Organization> GetAllOrganizationsByOrganizationManagerId(Guid organizationManagerId);

    }
}
