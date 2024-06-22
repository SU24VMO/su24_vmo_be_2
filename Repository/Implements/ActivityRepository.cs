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
    public class ActivityRepository : IActivityRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Activity> GetAll()
        {
            using var context = new VMODBContext();
            return context.Activities
                .Include(a => a.ProcessingPhase).ToList();
        }

        public Activity? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Activities
                .Include(a => a.ProcessingPhase).ToList()
                .FirstOrDefault(a => a.ActivityId.Equals(id));
        }

        public Activity? Save(Activity entity)
        {
            try
            {
                using var context = new VMODBContext();
                var activityCreated = context.Activities.Add(entity);
                context.SaveChanges();
                return activityCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Activity entity)
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