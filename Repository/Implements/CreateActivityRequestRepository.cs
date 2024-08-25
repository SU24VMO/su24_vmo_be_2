using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public IEnumerable<CreateActivityRequest> GetAllActivitiesRequestCreateByOM(Guid omId)
        {
            using var context = new VMODBContext();
            return context.CreateActivityRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .Include(a => a.Activity)
                .Where(c => c.CreateByOM != null && c.CreateByOM.Equals(omId))
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public IEnumerable<CreateActivityRequest> GetAllActivitiesRequestCreateByOM(Guid omId, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();

            // Get the list of requests filtered by memberId
            var query = context.CreateActivityRequests
                .Include(a => a.Activity)
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .Where(c => c.CreateByOM.Equals(omId))
                .OrderByDescending(a => a.CreateDate);

            // Calculate total count of the filtered list
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

        public IEnumerable<CreateActivityRequest> GetAllActivitiesRequestCreateByVolunteer(Guid memberId)
        {
            using var context = new VMODBContext();
            return context.CreateActivityRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .Include(a => a.Activity)
                .Where(c => c.CreateByMember != null && c.CreateByMember.Equals(memberId))
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public IEnumerable<CreateActivityRequest> GetAllActivitiesRequestCreateByVolunteer(Guid memberId, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();

            // Get the list of requests filtered by memberId
            var query = context.CreateActivityRequests
                .Include(a => a.Activity)
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .Where(c => c.CreateByMember.Equals(memberId))
                .OrderByDescending(a => a.CreateDate);

            // Calculate total count of the filtered list
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
    }
}
