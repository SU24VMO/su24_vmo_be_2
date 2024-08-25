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
        public IEnumerable<Post> GetAllPostByOrganizationManagerId(Guid organizationManagerId, int? pageSize, int? pageNo);
        public IEnumerable<Post> GetAllPostsByMemberId(Guid memberId);
        public IEnumerable<Post> GetAllPostsByMemberId(Guid memberId, int? pageSize, int? pageNo);
    }
}
