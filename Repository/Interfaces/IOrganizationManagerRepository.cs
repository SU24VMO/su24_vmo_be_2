using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IOrganizationManagerRepository : ICrudBaseRepository<OrganizationManager, Guid>
    {
        public OrganizationManager? GetByEmail(string email);

        public OrganizationManager? GetByAccountID(Guid accountID);
        public OrganizationManager? GetByPhone(string phone);
    }
}
