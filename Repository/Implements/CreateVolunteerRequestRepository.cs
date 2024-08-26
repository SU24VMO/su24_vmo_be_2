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
    public class CreateVolunteerRequestRepository : ICreateVolunteerRequestRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CreateVolunteerRequest> GetAll()
        {
            using var context = new VMODBContext();
            return context.CreateVolunteerRequests
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public CreateVolunteerRequest? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.CreateVolunteerRequests
                .Include(a => a.Member)
                .Include(a => a.Moderator).ToList()
                .FirstOrDefault(d => d.CreateVolunteerRequestID.Equals(id));
        }

        public IEnumerable<CreateVolunteerRequest> GetCreateVolunteerRequestsWithEmail(string email)
        {
            using var context = new VMODBContext();
            return context.CreateVolunteerRequests
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList()
                .Where(c => c.Email.ToLower().Equals(email.ToLower()));
        }

        public IEnumerable<CreateVolunteerRequest> GetCreateVolunteerRequestsWithPhoneNumber(string phoneNumber)
        {
            using var context = new VMODBContext();
            return context.CreateVolunteerRequests
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList()
                .Where(c => c.PhoneNumber.ToLower().Equals(phoneNumber.ToLower()));
        }

        public IEnumerable<CreateVolunteerRequest> GetAllCreateVolunteerRequestsByMemberName(string? memberName, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            var query = context.CreateVolunteerRequests
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList()
                .Where(m => (m.Member!.FirstName.Trim().ToLower() + " " + m.Member.LastName.Trim().ToLower()).Contains(memberName?.ToLower().Trim() ?? string.Empty));
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

        public IEnumerable<CreateVolunteerRequest> GetAllCreateVolunteerRequestsByMemberName(string? memberName)
        {
            using var context = new VMODBContext();
            var query = context.CreateVolunteerRequests
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList()
                .Where(m => (m.Member!.FirstName.Trim().ToLower() + " " + m.Member.LastName.Trim().ToLower()).Contains(memberName?.ToLower().Trim() ?? string.Empty));
            return query.ToList();
        }

        public CreateVolunteerRequest? Save(CreateVolunteerRequest entity)
        {
            try
            {
                using var context = new VMODBContext();
                entity.Email = entity.Email.ToLower();
                var userAdded = context.CreateVolunteerRequests.Add(entity);
                context.SaveChanges();
                return userAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(CreateVolunteerRequest entity)
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
