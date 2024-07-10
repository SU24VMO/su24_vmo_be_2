﻿using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class CreateOrganizationManagerRequestRepository : ICreateOrganizationManagerRequestRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CreateOrganizationManagerRequest> GetAll()
        {
            using var context = new VMODBContext();
            return context.CreateOrganizationManagerRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public CreateOrganizationManagerRequest? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.CreateOrganizationManagerRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Moderator).ToList().
                    FirstOrDefault(a => a.CreateOrganizationManagerRequestID.Equals(id));
        }

        public IEnumerable<CreateOrganizationManagerRequest> GetCreateOrganizationManagerRequestsWithEmail(string email)
        {
            using var context = new VMODBContext();
            return context.CreateOrganizationManagerRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList()
                .Where(c => c.Email.ToLower().Equals(email.ToLower()));
        }

        public IEnumerable<CreateOrganizationManagerRequest> GetCreateOrganizationManagerRequestsWithPhoneNumber(string phoneNumber)
        {
            using var context = new VMODBContext();
            return context.CreateOrganizationManagerRequests
                .Include(a => a.OrganizationManager)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList()
                .Where(c => c.PhoneNumber.ToLower().Equals(phoneNumber.ToLower()));
        }

        public CreateOrganizationManagerRequest? Save(CreateOrganizationManagerRequest entity)
        {
            using var context = new VMODBContext();
            try
            {
                var createCampaignRequest = context.CreateOrganizationManagerRequests.Add(entity);
                context.SaveChanges();
                return createCampaignRequest.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(CreateOrganizationManagerRequest entity)
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
