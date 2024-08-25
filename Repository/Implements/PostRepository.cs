using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class PostRepository : IPostRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetAll()
        {
            using var context = new VMODBContext();
            return context.Posts
                .Include(a => a.CreatePostRequest)
                .OrderByDescending(a => a.CreateAt).ToList();
        }

        public IEnumerable<Post> GetAllPostByOrganizationManagerId(Guid organizationManagerId)
        {
            using var context = new VMODBContext();
            var createPostRequests = context.CreatePostRequests.Include(a => a.Post).Where(a => a.CreateByOM.Equals(organizationManagerId)).ToList();
            var posts = new List<Post>();
            foreach (var post in createPostRequests)
            {
                if (post.Post != null)
                    posts.Add(post.Post!);
            }
            return posts.OrderByDescending(a => a.CreateAt);
        }

        public IEnumerable<Post> GetAllPostByOrganizationManagerId(Guid organizationManagerId, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            var createPostRequests = context.CreatePostRequests.Include(a => a.Post).Where(a => a.CreateByOM.Equals(organizationManagerId)).ToList();
            var posts = new List<Post>();
            foreach (var post in createPostRequests)
            {
                if (post.Post != null)
                    posts.Add(post.Post!);
            }
            var query = posts.OrderByDescending(a => a.CreateAt);
            int totalCount = query.Count();

            // Set pageSize to the total count if it's not provided
            int size = pageSize ?? totalCount;
            int page = pageNo ?? 1;

            // Apply pagination
            return query
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }

        public IEnumerable<Post> GetAllPostsByMemberId(Guid userId)
        {
            using var context = new VMODBContext();
            var createPostRequests = context.CreatePostRequests.Include(a => a.Post).Where(a => a.CreateByMember.Equals(userId)).ToList();
            var posts = new List<Post>();
            foreach (var post in createPostRequests)
            {
                if (post.Post != null)
                    posts.Add(post.Post!);
            }
            return posts.OrderByDescending(a => a.CreateAt);
        }

        public IEnumerable<Post> GetAllPostsByMemberId(Guid memberId, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            var createPostRequests = context.CreatePostRequests.Include(a => a.Post).Where(a => a.CreateByMember.Equals(memberId)).ToList();
            var posts = new List<Post>();
            foreach (var post in createPostRequests)
            {
                if (post.Post != null)
                    posts.Add(post.Post!);
            }
            var query = posts.OrderByDescending(a => a.CreateAt);
            int totalCount = query.Count();

            // Set pageSize to the total count if it's not provided
            int size = pageSize ?? totalCount;
            int page = pageNo ?? 1;

            // Apply pagination
            return query
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

        }

        public Post? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Posts
                .Include(a => a.CreatePostRequest).ToList()
                .FirstOrDefault(a => a.PostID.Equals(id));
        }

        public Post? Save(Post entity)
        {
            using var context = new VMODBContext();
            try
            {
                var postRequest = context.Posts.Add(entity);
                context.SaveChanges();
                return postRequest.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Post entity)
        {
            try
            {
                using var context = new VMODBContext();
                context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}
