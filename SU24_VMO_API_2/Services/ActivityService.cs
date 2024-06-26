using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class ActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IProcessingPhaseRepository _processingPhaseRepository;
        private readonly IActivityImageRepository _activityImageRepository;
        private readonly FirebaseService _firebaseService;

        public ActivityService(IActivityRepository activityRepository, IProcessingPhaseRepository processingPhaseRepository,
            FirebaseService firebaseService, IActivityImageRepository activityImageRepository)
        {
            _activityRepository = activityRepository;
            _processingPhaseRepository = processingPhaseRepository;
            _firebaseService = firebaseService;
            _activityImageRepository = activityImageRepository;
        }

        public IEnumerable<Activity> GetAll()
        {
            return _activityRepository.GetAll();
        }

        public Activity? GetById(Guid id)
        {
            return _activityRepository.GetById(id);
        }

        public async Task<Activity?> CreateActivity(CreateNewActivityRequest request)
        {
            TryValidateCreateActivityRequest(request);
            var activity = new Activity
            {
                ActivityId = Guid.NewGuid(),
                Content = request.Content,
                Title = request.Title,
                ProcessingPhaseId = request.ProcessingPhaseId,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsActive = true,
            };
            var activityCreated = _activityRepository.Save(activity);

            if (request.ActivityImages != null && activityCreated != null)
                foreach (var item in request.ActivityImages)
                {
                    var activityImage = new ActivityImage
                    {
                        ActivityImageId = Guid.NewGuid(),
                        ActivityId = activity.ActivityId,
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        Link = await _firebaseService.UploadImage(item),
                        IsActive = true,
                    };
                    _activityImageRepository.Save(activityImage);
                }

            return activityCreated;
        }

        public void UpdateActivity(UpdateActivityRequest request)
        {
            TryValidateUpdateActivityRequest(request);
            var activity = _activityRepository.GetById(request.ActivityId)!;
            if (!String.IsNullOrEmpty(activity.Title))
            {
                activity.Title = request.Title!.Trim();
            }
            if (!String.IsNullOrEmpty(activity.Content))
            {
                activity.Content = request.Content!.Trim();
            }
            activity.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
            _activityRepository.Update(activity);
        }

        private void TryValidateCreateActivityRequest(CreateNewActivityRequest request)
        {
            if (!String.IsNullOrEmpty(request.ProcessingPhaseId.ToString()))
            {
                throw new Exception("ProcessingPhaseId must not empty.");
            }

            if (_processingPhaseRepository.GetById(request.ProcessingPhaseId) == null)
            {
                throw new Exception("Processing phase not found.");
            }
            if (!String.IsNullOrEmpty(request.Content))
            {
                throw new Exception("Content is not empty.");
            }
            if (!String.IsNullOrEmpty(request.Title))
            {
                throw new Exception("Title is not empty.");
            }
        }


        private void TryValidateUpdateActivityRequest(UpdateActivityRequest request)
        {
            if (!String.IsNullOrEmpty(request.ActivityId.ToString()))
            {
                throw new Exception("ActivityId must not empty.");
            }

            if (_activityRepository.GetById(request.ActivityId) == null)
            {
                throw new Exception("Activity not found.");
            }

        }
    }
}
