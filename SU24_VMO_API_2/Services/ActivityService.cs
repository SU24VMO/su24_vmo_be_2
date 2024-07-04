using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Response;

namespace SU24_VMO_API.Services
{
    public class ActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IProcessingPhaseRepository _processingPhaseRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ICreateActivityRequestRepository _createActivityRequestRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IActivityImageRepository _activityImageRepository;
        private readonly FirebaseService _firebaseService;

        public ActivityService(IActivityRepository activityRepository, IProcessingPhaseRepository processingPhaseRepository,
            FirebaseService firebaseService, IActivityImageRepository activityImageRepository, IMemberRepository memberRepository,
            IOrganizationManagerRepository organizationManagerRepository, ICreateActivityRequestRepository createActivityRequestRepository,
            ICampaignRepository campaignRepository, IOrganizationRepository organizationRepository)
        {
            _activityRepository = activityRepository;
            _processingPhaseRepository = processingPhaseRepository;
            _firebaseService = firebaseService;
            _activityImageRepository = activityImageRepository;
            _memberRepository = memberRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _createActivityRequestRepository = createActivityRequestRepository;
            _campaignRepository = campaignRepository;
            _organizationRepository = organizationRepository;
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


        public IEnumerable<ActivityResponse?> GetAllActivityWhichCreateByVolunteer(Guid memberId)
        {
            var createActivityRequests = _createActivityRequestRepository.GetAll().Where(c => c.CreateByMember != null && c.CreateByMember.Equals(memberId));
            var activities = new List<ActivityResponse?>();
            foreach (var request in createActivityRequests)
            {
                if (request.Activity != null)
                {
                    var processingPhase = _processingPhaseRepository.GetById(request.Activity.ProcessingPhaseId);
                    if (processingPhase != null)
                    {
                        var campaign = _campaignRepository.GetById(processingPhase.CampaignId);
                        if (campaign != null && campaign.OrganizationID == null)
                        {
                            activities.Add(new ActivityResponse
                            {
                                ActivityId = request.ActivityID,
                                ActivityImages = request.Activity.ActivityImages,
                                Content = request.Activity.Content,
                                IsActive = request.Activity.IsActive,
                                CreateDate = request.Activity.CreateDate,
                                Title = request.Activity.Title,
                                ProcessingPhaseId = request.Activity.ProcessingPhaseId,
                                ProcessingPhase = request.Activity.ProcessingPhase,
                                UpdateDate = request.Activity.UpdateDate,
                                CampaignName = campaign.Name,
                                OrganizationName = null,
                            });
                        }
                    }
                }
            }
            return activities;
        }

        public IEnumerable<ActivityResponse?> GetAllActivityWhichCreateByOM(Guid omId)
        {
            var createActivityRequests = _createActivityRequestRepository.GetAll().Where(c => c.CreateByOM != null && c.CreateByOM.Equals(omId));
            var activities = new List<ActivityResponse?>();
            foreach (var request in createActivityRequests)
            {
                if (request.Activity != null)
                {
                    var processingPhase = _processingPhaseRepository.GetById(request.Activity.ProcessingPhaseId);
                    if (processingPhase != null)
                    {
                        var campaign = _campaignRepository.GetById(processingPhase.CampaignId);
                        if (campaign != null && campaign.OrganizationID != null)
                        {
                            var organization = _organizationRepository.GetById((Guid)campaign.OrganizationID);
                            if (organization != null)
                            {
                                activities.Add(new ActivityResponse
                                {
                                    ActivityId = request.ActivityID,
                                    ActivityImages = request.Activity.ActivityImages,
                                    Content = request.Activity.Content,
                                    IsActive = request.Activity.IsActive,
                                    CreateDate = request.Activity.CreateDate,
                                    Title = request.Activity.Title,
                                    ProcessingPhaseId = request.Activity.ProcessingPhaseId,
                                    ProcessingPhase = request.Activity.ProcessingPhase,
                                    UpdateDate = request.Activity.UpdateDate,
                                    CampaignName = campaign.Name,
                                    OrganizationName = organization.Name,
                                });
                            }
                        }
                    }
                }
            }
            return activities;
        }


        public IEnumerable<Activity> GetAllActivityWithProcessingPhaseId(Guid processingId)
        {
            var activies = _activityRepository.GetAll().Where(a => a.ProcessingPhaseId.Equals(processingId));
            return activies;
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
