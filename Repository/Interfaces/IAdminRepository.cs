using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IAdminRepository : ICrudBaseRepository<Admin, Guid>
    {
        public Admin? GetByEmail(string email);
        public Admin? GetByAccountID(Guid accountID);
    }
}
