using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICreateVolunteerRequestRepository : ICrudBaseRepository<CreateVolunteerRequest, Guid>
    {
        public IEnumerable<CreateVolunteerRequest> GetCreateVolunteerRequestsWithEmail(string email);
        public IEnumerable<CreateVolunteerRequest> GetCreateVolunteerRequestsWithPhoneNumber(string phoneNumber);

    }
}
