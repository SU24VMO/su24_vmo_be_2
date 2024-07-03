using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IMemberRepository : ICrudBaseRepository<Member, Guid>
    {
        public Member? GetByAccountId(Guid? accountId);
        public Member? GetByPhone(string phone);
    }
}
