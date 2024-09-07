using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IActivityRepository : ICrudBaseRepository<Activity, Guid>
    {
        public Task<Activity?> GetByIdAsync(Guid id);
        public IEnumerable<Activity> GetActivitiesByProcessingPhaseId(Guid processingPhaseId);
    }
}
