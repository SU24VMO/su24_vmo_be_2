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
    public class RequestManagerRepository : IRequestManagerRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RequestManager> GetAll()
        {
            using var context = new VMODBContext();
            return context.RequestManagers
                .Include(a => a.CreateMemberRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateOrganizationManagerRequests)
                .Include(a => a.CreateCampaignRequests).ToList();
        }

        public RequestManager? GetByAccountID(Guid accountID)
        {
            using var context = new VMODBContext();
            return context.RequestManagers
                .Include(a => a.CreateMemberRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateOrganizationManagerRequests)
                .Include(a => a.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.AccountID.Equals(accountID));
        }

        public RequestManager? GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public RequestManager? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.RequestManagers
                .Include(a => a.CreateMemberRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateOrganizationManagerRequests)
                .Include(a => a.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.AccountID.Equals(id));
        }

        public RequestManager? GetByPhone(string phone)
        {
            using var context = new VMODBContext();
            return context.RequestManagers
                .Include(a => a.CreateMemberRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateOrganizationManagerRequests)
                .Include(a => a.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.PhoneNumber.Equals(phone));
        }

        public RequestManager? Save(RequestManager entity)
        {
            try
            {
                using var context = new VMODBContext();
                var requestManagerCreated = context.RequestManagers.Add(entity);
                context.SaveChanges();
                return requestManagerCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(RequestManager entity)
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
