using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IPostRepository : ICrudBaseRepository<Post, Guid>
    {
        public IEnumerable<Post> GetAllPostByOrganizationManagerId(Guid organizationManagerId);
        public IEnumerable<Post> GetAllPostsByMemberId(Guid memberId);

    }
}
