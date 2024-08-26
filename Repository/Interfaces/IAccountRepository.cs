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

        public IEnumerable<Account> GetAllAccountsWithVolunteerRole(string? name);
        public IEnumerable<Account> GetAllAccountsWithVolunteerRole(string? name, int? pageSize, int? pageNo);

        public IEnumerable<Account> GetAllAccountWithMemberRole(string? name);
        public IEnumerable<Account> GetAllAccountWithMemberRole(string? name, int? pageSize, int? pageNo);

        public IEnumerable<Account> GetAllAccountsWithModeratorRole(string? name);
        public IEnumerable<Account> GetAllAccountsWithModeratorRole(string? name, int? pageSize, int? pageNo);

        public IEnumerable<Account> GetAllAccountsWithOrganizationManagerRole(string? name);
        public IEnumerable<Account> GetAllAccountsWithOrganizationManagerRole(string? name, int? pageSize, int? pageNo);

        public Task<Account?> GetByIdAsync(Guid id);

    }
}
