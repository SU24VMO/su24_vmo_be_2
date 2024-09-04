using BusinessObject.Models;
using Repository.Interfaces;

namespace SU24_VMO_API_2.Services
{
    public class ActivityStatementFileService
    {
        private readonly IActivityStatementFileRepository _activityStatementFileRepository;

        public ActivityStatementFileService(IActivityStatementFileRepository activityStatementFileRepository)
        {
            _activityStatementFileRepository = activityStatementFileRepository;
        }

        public IEnumerable<ActivityStatementFile> GetAllActivityStatementFiles()
        {
            return _activityStatementFileRepository.GetAll();
        }

        public ActivityStatementFile? GetActivityStatementFileById(Guid id)
        {
            return _activityStatementFileRepository.GetById(id);
        }
    }
}
