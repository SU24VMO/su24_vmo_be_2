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
    public class CreateMemberRequestRepository : ICreateMemberRequestRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CreateMemberRequest> GetAll()
        {
            using var context = new VMODBContext();
            return context.CreateMemberRequests
                .Include(a => a.User)
                .Include(a => a.RequestManager).ToList();
        }

        public CreateMemberRequest? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.CreateMemberRequests
                .Include(a => a.User)
                .Include(a => a.RequestManager).ToList()
                .FirstOrDefault(d => d.CreateMemberRequestID.Equals(id));
        }

        public CreateMemberRequest? Save(CreateMemberRequest entity)
        {
            try
            {
                using var context = new VMODBContext();
                entity.Email = entity.Email.ToLower();
                var userAdded = context.CreateMemberRequests.Add(entity);
                context.SaveChanges();
                return userAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(CreateMemberRequest entity)
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
