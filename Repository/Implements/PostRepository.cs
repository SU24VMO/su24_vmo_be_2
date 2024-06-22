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
                .Include(a => a.CreatePostRequest).ToList();
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
