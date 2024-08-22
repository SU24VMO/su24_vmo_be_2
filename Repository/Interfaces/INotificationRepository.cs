using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface INotificationRepository : ICrudBaseRepository<Notification, Guid>
    {
        public IEnumerable<Notification> GetAllNotificationsByAccountId(Guid accountId);
        public Task<Notification?> SaveAsync(Notification entity);

    }
}
