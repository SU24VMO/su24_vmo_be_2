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
    public class ModeratorRepository : IModeratorRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Moderator> GetAll()
        {
            using var context = new VMODBContext();
            return context.Moderators
                .Include(a => a.CreateVolunteerRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateOrganizationManagerRequests)
                .Include(a => a.CreateCampaignRequests).ToList();
        }

        public Moderator? GetByAccountID(Guid accountID)
        {
            using var context = new VMODBContext();
            return context.Moderators
                .Include(a => a.CreateVolunteerRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateOrganizationManagerRequests)
                .Include(a => a.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.AccountID.Equals(accountID));
        }

        public Moderator? GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Moderator? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Moderators
                .Include(a => a.CreateVolunteerRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateOrganizationManagerRequests)
                .Include(a => a.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.ModeratorID.Equals(id));
        }

        public Moderator? GetByPhone(string phone)
        {
            using var context = new VMODBContext();
            return context.Moderators
                .Include(a => a.CreateVolunteerRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateOrganizationManagerRequests)
                .Include(a => a.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.PhoneNumber.Equals(phone));
        }

        public Moderator? Save(Moderator entity)
        {
            try
            {
                using var context = new VMODBContext();
                var requestManagerCreated = context.Moderators.Add(entity);
                context.SaveChanges();
                return requestManagerCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Moderator entity)
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
