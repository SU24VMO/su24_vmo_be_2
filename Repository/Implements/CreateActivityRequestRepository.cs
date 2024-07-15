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
    public class CreateActivityRequestRepository : ICreateActivityRequestRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CreateActivityRequest> GetAll()
        {
            using var context = new VMODBContext();
            return context.CreateActivityRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .Include(a => a.Activity)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public CreateActivityRequest? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.CreateActivityRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .Include(a => a.Activity).ToList()
                .FirstOrDefault(a => a.CreateActivityRequestID.Equals(id));
        }

        public CreateActivityRequest? Save(CreateActivityRequest entity)
        {
            try
            {
                using var context = new VMODBContext();
                var createActivityRequestCreated = context.CreateActivityRequests.Add(entity);
                context.SaveChanges();
                return createActivityRequestCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(CreateActivityRequest entity)
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

        public CreateActivityRequest? GetCreateActivityRequestByActivityId(Guid activityId)
        {
            using var context = new VMODBContext();
            return context.CreateActivityRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .Include(a => a.Activity).ToList()
                .FirstOrDefault(a => a.ActivityID.Equals(activityId));
        }
    }
}
