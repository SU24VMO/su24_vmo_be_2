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
    public class CreatePostRequestRepository : ICreatePostRequestRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CreatePostRequest> GetAll()
        {
            using var context = new VMODBContext();
            return context.CreatePostRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Post)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public CreatePostRequest? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.CreatePostRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Post)
                .Include(a => a.Member)
                .Include(a => a.Moderator).ToList()
                .FirstOrDefault(a => a.CreatePostRequestID.Equals(id));
        }

        public CreatePostRequest? Save(CreatePostRequest entity)
        {
            using var context = new VMODBContext();
            try
            {
                var createPostRequest = context.CreatePostRequests.Add(entity);
                context.SaveChanges();
                return createPostRequest.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(CreatePostRequest entity)
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
