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
    public class CreateOrganizationRequestRepository : ICreateOrganizationRequestRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CreateOrganizationRequest> GetAll()
        {
            using var context = new VMODBContext();
            return context.CreateOrganizationRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public CreateOrganizationRequest? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.CreateOrganizationRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Moderator)
                .Include(a => a.Organization).ToList()
                .FirstOrDefault(a => a.CreateOrganizationRequestID.Equals(id));
        }

        public CreateOrganizationRequest? Save(CreateOrganizationRequest entity)
        {
            using var context = new VMODBContext();
            try
            {
                var createOrganizationRequest = context.CreateOrganizationRequests.Add(entity);
                context.SaveChanges();
                return createOrganizationRequest.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(CreateOrganizationRequest entity)
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
