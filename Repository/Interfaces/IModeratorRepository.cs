using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IModeratorRepository : ICrudBaseRepository<Moderator, Guid>
    {
        public Moderator? GetByEmail(string email);

        public Moderator? GetByAccountID(Guid accountID);
        public Moderator? GetByPhone(string phone);

    }
}
