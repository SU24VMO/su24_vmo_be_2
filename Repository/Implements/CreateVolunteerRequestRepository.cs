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