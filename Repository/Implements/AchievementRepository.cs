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
    public class AchievementRepository : IAchievementRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Achievement> GetAll()
        {
            using var context = new VMODBContext();
            return context.Achievements
                .Include(a => a.Organization)
                .OrderByDescending(a => a.CreatedDate).ToList();
        }

        public Achievement? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Achievements
                .Include(a => a.Organization)
                .OrderByDescending(a => a.CreatedDate).ToList()
                .FirstOrDefault(a => a.AchievementID.Equals(id));
        }

        public Achievement? Save(Achievement entity)
        {
            try
            {
                using var context = new VMODBContext();
                var achievementCreated = context.Achievements.Add(entity);
                context.SaveChanges();
                return achievementCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Achievement entity)
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
