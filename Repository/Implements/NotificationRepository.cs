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
    public class NotificationRepository : INotificationRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Notification> GetAll()
        {
            using var context = new VMODBContext();
            return context.Notifications
                .Include(a => a.Account).ToList();
        }

        public IEnumerable<Notification> GetAllNotificationsByAccountId(Guid accountId)
        {
            using var context = new VMODBContext();
            return context.Notifications
                .Include(a => a.Account).Where(a => a.AccountID.Equals(accountId)).ToList();
        }

        public Notification? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Notifications
                .Include(a => a.Account).FirstOrDefault(n => n.AccountID.Equals(id));
        }

        public Notification? Save(Notification entity)
        {
            try
            {
                using var context = new VMODBContext();
                var notiAdded = context.Notifications.Add(entity);
                context.SaveChanges();
                return notiAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Notification entity)
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
