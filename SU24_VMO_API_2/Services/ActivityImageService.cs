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
        private readonly FirebaseService _firebaseService;

        public ActivityImageService(IActivityImageRepository activityImageRepository, IActivityRepository activityRepository, 
            FirebaseService firebaseService)
        {
            _activityImageRepository = activityImageRepository;
            _activityRepository = activityRepository;
            _firebaseService = firebaseService;
        }
        public IEnumerable<ActivityImage> GetAll()
        {
            return _activityImageRepository.GetAll();
        }

        public ActivityImage? GetById(Guid id)
        {
            return _activityImageRepository.GetById(id);
        }

        public async Task<string?> CreateActivityImage(CreateActivityImageRequest request)
        {
            TryValidateCreateActivityImageRequest(request);
            foreach(var image in request.Images)
            {
                var activityImage = new ActivityImage
                {
                    ActivityImageId = Guid.NewGuid(),
                    ActivityId = request.ActivityId,
                    Link = await _firebaseService.UploadImage(image),
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    IsActive = true
                };
                var activityImageCreated = _activityImageRepository.Save(activityImage);
            }

            return "Đã đăng hình thành công!";
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
            if (request.Images == null)
            {
                throw new Exception("Hình không được để trống");
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
