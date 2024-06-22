using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IRequestManagerRepository : ICrudBaseRepository<RequestManager, Guid>
    {
        public RequestManager? GetByEmail(string email);

        public RequestManager? GetByAccountID(Guid accountID);
        public RequestManager? GetByPhone(string phone);

    }
}
