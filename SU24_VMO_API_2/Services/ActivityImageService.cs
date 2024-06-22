using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class ActivityImageService
    {
        private readonly IActivityImageRepository _activityImageRepository;
        private readonly IActivityRepository _activityRepository;

        public ActivityImageService(IActivityImageRepository activityImageRepository, IActivityRepository activityRepository)
        {
            _activityImageRepository = activityImageRepository;
            _activityRepository = activityRepository;
        }
        public IEnumerable<ActivityImage> GetAll()
        {
            return _activityImageRepository.GetAll();
        }

        public ActivityImage? GetById(Guid id)
        {
            return _activityImageRepository.GetById(id);
        }

        public ActivityImage? CreateActivityImage(CreateActivityImageRequest request)
        {
            TryValidateCreateActivityImageRequest(request);

            var activityImage = new ActivityImage
            {
                ActivityImageId = Guid.NewGuid(),
                ActivityId = request.ActivityId,
                Link = request.Link,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsActive = true
            };
            var activityImageCreated = _activityImageRepository.Save(activityImage);
            return activityImageCreated;
        }

        public void UpdateActivityImage(UpdateActivityImageRequest request)
        {
            TryValidateUpdateActivityImageRequest(request);

            var activityImage = _activityImageRepository.GetById(request.ActivityImageId)!;
            if (!String.IsNullOrEmpty(request.Link))
            {
                activityImage.Link = request.Link;
            }
            if (request.IsActive != null)
            {
                activityImage.IsActive = (bool)request.IsActive;
            }

            activityImage.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);

            _activityImageRepository.Update(activityImage);
        }


        private void TryValidateCreateActivityImageRequest(CreateActivityImageRequest request)
        {
            if (!String.IsNullOrEmpty(request.ActivityId.ToString()))
            {
                throw new Exception("ActivityId must not empty.");
            }

            if (_activityRepository.GetById(request.ActivityId) == null)
            {
                throw new Exception("Activity not found.");
            }
            if (!String.IsNullOrEmpty(request.Link))
            {
                throw new Exception("Description is not empty.");
            }
        }


        private void TryValidateUpdateActivityImageRequest(UpdateActivityImageRequest request)
        {
            if (!String.IsNullOrEmpty(request.ActivityImageId.ToString()))
            {
                throw new Exception("ActivityImageId must not empty.");
            }

            if (_activityImageRepository.GetById(request.ActivityImageId) == null)
            {
                throw new Exception("Activity image not found.");
            }
            if (!String.IsNullOrEmpty(request.Link))
            {
                throw new Exception("Link is not empty.");
            }
        }
    }
}
