using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUserRepository : ICrudBaseRepository<User, Guid>
    {
        public User? GetByAccountId(Guid? accountId);
        public User? GetByPhone(string phone);
    }
}
