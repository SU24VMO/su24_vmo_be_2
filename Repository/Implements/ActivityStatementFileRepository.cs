using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implements
{
    public class ActivityStatementFileRepository : IActivityStatementFileRepository
    {
        public IEnumerable<ActivityStatementFile> GetAll()
        {
            using var context = new VMODBContext();
            return context.ActivityStatementFiles
                .Include(a => a.Activity)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public ActivityStatementFile? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.ActivityStatementFiles
                .Include(a => a.Activity).ToList()
                .FirstOrDefault(a => a.ActivityStatementFileId.Equals(id));
        }

        public ActivityStatementFile? Save(ActivityStatementFile entity)
        {
            try
            {
                using var context = new VMODBContext();
                var activityStatementFileAdded = context.ActivityStatementFiles.Add(entity);
                context.SaveChanges();
                return activityStatementFileAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void DeleteById(Guid id)
        {
            using var context = new VMODBContext();
            var activityStatementFile = context.ActivityStatementFiles.FirstOrDefault(ac => ac.ActivityStatementFileId.Equals(id));
            if (activityStatementFile != null)
            {
                context.ActivityStatementFiles.Remove(activityStatementFile);
                context.SaveChanges();
            }
        }

        public void Update(ActivityStatementFile entity)
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
