using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICreatePostRequestRepository : ICrudBaseRepository<CreatePostRequest, Guid>
    {
        public CreatePostRequest GetCreatePostRequestByPostId(Guid postId);
        public IEnumerable<CreatePostRequest>? GetAllCreatePostRequestsByPostTitle(string? postTitle);
        public IEnumerable<CreatePostRequest>? GetAllCreatePostRequestsByPostTitle(string? postTitle, int? pageSize, int? pageNo);


    }
}
