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
    public class OrganizationRepository : IOrganizationRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Organization> GetAll()
        {
            using var context = new VMODBContext();
            return context.Organizations
                .Include(a => a.OrganizationManager)
                .Include(a => a.Achievements)
                .Include(a => a.Campaigns)
                .OrderByDescending(a => a.CreatedAt).ToList();
        }

        public IEnumerable<Organization> GetAllOrganizationsByOrganizationManagerId(Guid organizationManagerId)
        {
            using var context = new VMODBContext();
            return context.Organizations
                .Include(a => a.Achievements)
                .Include(a => a.OrganizationManager)
                .Include(a => a.Campaigns)
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.OrganizationManagerID.Equals(organizationManagerId));
        }

        public Organization? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Organizations
                .Include(a => a.OrganizationManager)
                .Include(a => a.Achievements)
                .Include(a => a.Campaigns).ToList()
                .FirstOrDefault(a => a.OrganizationID.Equals(id));
        }

        public IEnumerable<Organization> GetOrganizationsByOrganizationName(string organizationName)
        {
            using var context = new VMODBContext();
            return context.Organizations
                .Include(a => a.Achievements)
                .Include(a => a.OrganizationManager)
                .Include(a => a.Campaigns)
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.Name!.ToLower().Contains(organizationName.Trim().ToLower()));
        }

        public Organization? Save(Organization entity)
        {
            using var context = new VMODBContext();
            try
            {
                var organizationRequest = context.Organizations.Add(entity);
                context.SaveChanges();
                return organizationRequest.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Organization entity)
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
