using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Response;
using System.Text;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API_2.DTOs.Request;

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

        public IEnumerable<Activity> GetAllWithActivityTitle(string? title)
        {
            if (!String.IsNullOrEmpty(title))
                return _activityRepository.GetAll().Where(a => a.Title.ToLower().Contains(title.ToLower().Trim()));
            else return _activityRepository.GetAll();
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


        public ActivityCreateByVolunteer? GetAllActivityWhichCreateByVolunteer(Guid memberId, string? activityTitle, int? pageSize, int? pageNo)
        {
            if (!string.IsNullOrEmpty(activityTitle))
            {
                string normalizedActivityTitle = activityTitle.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
                var createActivityRequests =
                    _createActivityRequestRepository.GetAllActivitiesRequestCreateByVolunteer(memberId, pageSize,
                        pageNo);
                var activities = new List<ActivityResponse?>();
                foreach (var request in createActivityRequests)
                {
                    if (request.OrganizationManager != null)
                    {
                        request.OrganizationManager.CreateActivityRequests = null;
                        request.OrganizationManager.CreateCampaignRequests = null;
                        request.OrganizationManager.CreateOrganizationRequests = null;
                        request.OrganizationManager.CreatePostRequests = null;
                    }

                    if (request.Moderator != null)
                    {
                        request.Moderator.CreateActivityRequests = null;
                        request.Moderator.CreateCampaignRequests = null;
                        request.Moderator.CreateOrganizationRequests = null;
                        request.Moderator.CreatePostRequests = null;
                        request.Moderator.CreateOrganizationManagerRequests = null;
                        request.Moderator.CreateVolunteerRequests = null;
                    }
                    if (request.Activity != null)
                    {
                        var processingPhase = _processingPhaseRepository.GetById(request.Activity.ProcessingPhaseId);
                        if (processingPhase != null)
                        {
                            processingPhase.Campaign = null;
                            processingPhase.Activities = null;
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
                                    IsDisable = request.Activity.IsDisable,
                                    ProcessingPhaseId = request.Activity.ProcessingPhaseId,
                                    ProcessingPhase = processingPhase,
                                    UpdateDate = request.Activity.UpdateDate,
                                    CampaignName = campaign.Name,
                                    CampaignTier = campaign.CampaignTier,
                                    OrganizationName = null,
                                    CreateActivityRequest = request
                                });
                            }
                        }
                    }
                }

                return new ActivityCreateByVolunteer
                {
                    Activities = activities.Where(a => a.Title.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedActivityTitle) && a.IsDisable == false).ToList(),
                    TotalItem = _createActivityRequestRepository.GetAllActivitiesRequestCreateByVolunteer(memberId).Count()
                };
            }
            else
            {
                var createActivityRequests = _createActivityRequestRepository.GetAll().Where(c => c.CreateByMember != null && c.CreateByMember.Equals(memberId));
                var activities = new List<ActivityResponse?>();
                foreach (var request in createActivityRequests)
                {
                    if (request.OrganizationManager != null)
                    {
                        request.OrganizationManager.CreateActivityRequests = null;
                        request.OrganizationManager.CreateCampaignRequests = null;
                        request.OrganizationManager.CreateOrganizationRequests = null;
                        request.OrganizationManager.CreatePostRequests = null;
                    }

                    if (request.Moderator != null)
                    {
                        request.Moderator.CreateActivityRequests = null;
                        request.Moderator.CreateCampaignRequests = null;
                        request.Moderator.CreateOrganizationRequests = null;
                        request.Moderator.CreatePostRequests = null;
                        request.Moderator.CreateOrganizationManagerRequests = null;
                        request.Moderator.CreateVolunteerRequests = null;
                    }
                    if (request.Activity != null)
                    {
                        var processingPhase = _processingPhaseRepository.GetById(request.Activity.ProcessingPhaseId);
                        if (processingPhase != null)
                        {
                            processingPhase.Campaign = null;
                            processingPhase.Activities = null;
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
                                    IsDisable = request.Activity.IsDisable,
                                    ProcessingPhaseId = request.Activity.ProcessingPhaseId,
                                    ProcessingPhase = processingPhase,
                                    UpdateDate = request.Activity.UpdateDate,
                                    CampaignName = campaign.Name,
                                    CampaignTier = campaign.CampaignTier,
                                    OrganizationName = null,
                                    CreateActivityRequest = request
                                });
                            }
                        }
                    }
                }
                return new ActivityCreateByVolunteer
                {
                    Activities = activities.Where(c => c.IsDisable == false).ToList(),
                    TotalItem = _createActivityRequestRepository.GetAllActivitiesRequestCreateByVolunteer(memberId).Count()
                };
            }
        }

        public ActivityCreateByOM? GetAllActivityWhichCreateByOM(Guid omId, string? activityTitle, int? pageSize, int? pageNo)
        {
            if (!string.IsNullOrEmpty(activityTitle))
            {
                string normalizedActivityTitle = activityTitle.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
                var createActivityRequests = _createActivityRequestRepository.GetAllActivitiesRequestCreateByOM(omId, pageSize, pageNo);
                var activities = new List<ActivityResponse?>();
                foreach (var request in createActivityRequests)
                {
                    if (request.OrganizationManager != null)
                    {
                        request.OrganizationManager.CreateActivityRequests = null;
                        request.OrganizationManager.CreateCampaignRequests = null;
                        request.OrganizationManager.CreateOrganizationRequests = null;
                        request.OrganizationManager.CreatePostRequests = null;
                    }

                    if (request.Moderator != null)
                    {
                        request.Moderator.CreateActivityRequests = null;
                        request.Moderator.CreateCampaignRequests = null;
                        request.Moderator.CreateOrganizationRequests = null;
                        request.Moderator.CreatePostRequests = null;
                        request.Moderator.CreateOrganizationManagerRequests = null;
                        request.Moderator.CreateVolunteerRequests = null;
                    }
                    if (request.Activity != null)
                    {
                        var processingPhase = _processingPhaseRepository.GetById(request.Activity.ProcessingPhaseId);
                        if (processingPhase != null)
                        {
                            processingPhase.Campaign = null;
                            processingPhase.Activities = null;
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
                                        ProcessingPhase = processingPhase,
                                        IsDisable = request.Activity.IsDisable,
                                        UpdateDate = request.Activity.UpdateDate,
                                        CampaignName = campaign.Name,
                                        CampaignTier = campaign.CampaignTier,
                                        OrganizationName = organization.Name,
                                        CreateActivityRequest = request
                                    });
                                }
                            }
                        }
                    }
                }
                return new ActivityCreateByOM
                {
                    Activities = activities.Where(a => a.Title.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedActivityTitle) && a.IsDisable == false).ToList(),
                    TotalItem = _createActivityRequestRepository.GetAllActivitiesRequestCreateByOM(omId).Count()
                };
            }
            else
            {
                var createActivityRequests = _createActivityRequestRepository.GetAll().Where(c => c.CreateByOM != null && c.CreateByOM.Equals(omId));
                var activities = new List<ActivityResponse?>();
                foreach (var request in createActivityRequests)
                {
                    if (request.OrganizationManager != null)
                    {
                        request.OrganizationManager.CreateActivityRequests = null;
                        request.OrganizationManager.CreateCampaignRequests = null;
                        request.OrganizationManager.CreateOrganizationRequests = null;
                        request.OrganizationManager.CreatePostRequests = null;
                    }

                    if (request.Moderator != null)
                    {
                        request.Moderator.CreateActivityRequests = null;
                        request.Moderator.CreateCampaignRequests = null;
                        request.Moderator.CreateOrganizationRequests = null;
                        request.Moderator.CreatePostRequests = null;
                        request.Moderator.CreateOrganizationManagerRequests = null;
                        request.Moderator.CreateVolunteerRequests = null;
                    }
                    if (request.Activity != null)
                    {
                        var processingPhase = _processingPhaseRepository.GetById(request.Activity.ProcessingPhaseId);
                        if (processingPhase != null)
                        {
                            processingPhase.Campaign = null;
                            processingPhase.Activities = null;
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
                                        ProcessingPhase = processingPhase,
                                        IsDisable = request.Activity.IsDisable,
                                        UpdateDate = request.Activity.UpdateDate,
                                        CampaignName = campaign.Name,
                                        CampaignTier = campaign.CampaignTier,
                                        OrganizationName = organization.Name,
                                        CreateActivityRequest = request
                                    });
                                }
                            }
                        }
                    }
                }
                return new ActivityCreateByOM
                {
                    Activities = activities.Where(a => a.IsDisable == false).ToList(),
                    TotalItem = _createActivityRequestRepository.GetAllActivitiesRequestCreateByOM(omId).Count()
                };
            }
        }


        public IEnumerable<Activity> GetAllActivityWithProcessingPhaseId(Guid processingId)
        {
            var activies = _activityRepository.GetAll().Where(a => a.ProcessingPhaseId.Equals(processingId));
            return activies;
        }





        public async void UpdateActivity(Guid activityId, UpdateActivityRequest request)
        {
            TryValidateUpdateActivityRequest(activityId, request);
            var activity = _activityRepository.GetById(activityId)!;

            var activityRequest = _createActivityRequestRepository.GetCreateActivityRequestByActivityId(activityId);
            if (activityRequest != null)
            {
                if (activityRequest.IsApproved)
                {
                    throw new BadRequestException(
                        "Hoạt động này hiện đã được duyệt, vì vậy mọi thông tin của hoạt động này không thể chỉnh sửa!");
                }
            }
            if (!String.IsNullOrEmpty(activity.Title))
            {
                activity.Title = request.Title!.Trim();
            }
            if (!String.IsNullOrEmpty(activity.Content))
            {
                activity.Content = request.Content!.Trim();
            }

            if (request.ActivityImages != null)
            {
                foreach (var image in request.ActivityImages)
                {
                    var activityImages = _activityImageRepository.GetAllActivityImagesByActivityId(activityId);
                    foreach (var imageExisted in activityImages)
                    {
                        _activityImageRepository.DeleteById(imageExisted.ActivityImageId);
                    }

                    var activityImageCreate = _activityImageRepository.Save(new ActivityImage
                    {
                        ActivityId = activityId,
                        ActivityImageId = Guid.NewGuid(),
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsActive = true,
                        Link = await _firebaseService.UploadImage(image)
                    });
                }
            }
            activity.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
            _activityRepository.Update(activity);
        }


        public void UpdateStatusActivity(UpdateActivityDisable request)
        {
            var activity = _activityRepository.GetById(request.ActivityId)!;
            if (activity == null)
            {
                throw new NotFoundException("Không tìm thấy hoạt động này!");
            }

            var activityRequest = _createActivityRequestRepository.GetCreateActivityRequestByActivityId(request.ActivityId);
            if (activityRequest != null)
            {
                if (activityRequest.IsApproved)
                    throw new BadRequestException(
                        "Hoạt động này hiện đã được duyệt, vì vậy mọi thông tin của hoạt động này không thể chỉnh sửa!");
            }

            if (request.IsDisable)
            {
                activity.IsActive = false;
                activity.IsDisable = true;
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
                throw new Exception("Giai đoạn giải ngân không tìm thấy!.");
            }
            if (!String.IsNullOrEmpty(request.Content))
            {
                throw new Exception("Nội dung không được để trống.");
            }
            if (!String.IsNullOrEmpty(request.Title))
            {
                throw new Exception("Tiêu đề không được để trống.");
            }
        }


        private void TryValidateUpdateActivityRequest(Guid activityId, UpdateActivityRequest request)
        {
            if (!String.IsNullOrEmpty(activityId.ToString()))
            {
                throw new BadRequestException("Không được để trống trường này.");
            }

            if (_activityRepository.GetById(activityId) == null)
            {
                throw new NotFoundException("Không tìm thấy hoạt động này!");
            }

        }
    }
}
