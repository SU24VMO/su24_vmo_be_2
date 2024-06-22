using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IAccountRepository : ICrudBaseRepository<Account, Guid>
    {
        public Account? GetByEmail(string email);
        public Account? GetByUsername(string username);

        public IEnumerable<Account> GetAllAccountsWithMemberRole();
        public IEnumerable<Account> GetAllAccountsWithUserRole();
        public IEnumerable<Account> GetAllAccountsWithRequestManagerRole();
        public IEnumerable<Account> GetAllAccountsWithOrganizationManagerRole();
    }
}
