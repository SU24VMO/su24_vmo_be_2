using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICreateActivityRequestRepository : ICrudBaseRepository<CreateActivityRequest, Guid>
    {
        public CreateActivityRequest? GetCreateActivityRequestByActivityId(Guid activityId);
        public IEnumerable<CreateActivityRequest> GetAllActivitiesRequestCreateByOM(Guid omId);
        public IEnumerable<CreateActivityRequest> GetAllActivitiesRequestCreateByOM(Guid omId, int? pageSize, int? pageNo);
        public IEnumerable<CreateActivityRequest> GetAllActivitiesRequestCreateByVolunteer(Guid memberId);
        public IEnumerable<CreateActivityRequest> GetAllActivitiesRequestCreateByVolunteer(Guid memberId, int? pageSize, int? pageNo);
    }
}
