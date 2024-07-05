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
    public class ActivityImageRepository : IActivityImageRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ActivityImage> GetAll()
        {
            using var context = new VMODBContext();
            return context.ActivityImages
                .Include(a => a.Activity)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public IEnumerable<ActivityImage> GetAllActivityImagesByActivityId(Guid activityId)
        {
            using var context = new VMODBContext();
            return context.ActivityImages
                .Where(a => a.ActivityId.Equals(activityId))
                .Include(a => a.Activity)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public ActivityImage? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.ActivityImages
                .Include(a => a.Activity).ToList()
                .FirstOrDefault(a => a.ActivityImageId.Equals(id));
        }

        public ActivityImage? Save(ActivityImage entity)
        {
            try
            {
                using var context = new VMODBContext();
                var activityImageCreated = context.ActivityImages.Add(entity);
                context.SaveChanges();
                return activityImageCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(ActivityImage entity)
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
