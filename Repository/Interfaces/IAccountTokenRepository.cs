using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IAccountTokenRepository : ICrudBaseRepository<AccountToken, Guid>
    {
        public AccountToken? CheckRefreshToken(string code);
    }
}
