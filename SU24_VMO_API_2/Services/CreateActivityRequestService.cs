using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class CreateActivityRequestService
    {
        private readonly ICreateActivityRequestRepository _repository;
        private readonly IProcessingPhaseRepository _phaseRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IModeratorRepository _moderatorRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IActivityImageRepository _activityImageRepository;
        private readonly FirebaseService _firebaseService;

        public CreateActivityRequestService(ICreateActivityRequestRepository repository, IProcessingPhaseRepository phaseRepository, 
            IAccountRepository accountRepository, IOrganizationManagerRepository organizationManagerRepository, IActivityRepository activityRepository, 
            IMemberRepository memberRepository, INotificationRepository notificationRepository,
            IModeratorRepository moderatorRepository, IActivityImageRepository activityImageRepository, FirebaseService firebaseService)
        {
            _repository = repository;
            _phaseRepository = phaseRepository;
            _accountRepository = accountRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _activityRepository = activityRepository;
            _memberRepository = memberRepository;
            _notificationRepository = notificationRepository;
            _moderatorRepository = moderatorRepository;
            _activityImageRepository = activityImageRepository;
            _firebaseService = firebaseService;
        }

        public IEnumerable<CreateActivityRequest> GetAll()
        {
            return _repository.GetAll();
        }

        public CreateActivityRequest? GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public CreateActivityRequest? Save(CreateActivityRequest entity)
        {
            return _repository.Save(entity);
        }

        public void Update(CreateActivityRequest entity)
        {
            _repository.Update(entity);
        }

        public async Task<CreateActivityRequest?> CreateActivityRequestAsync(Guid accountId, CreateActivityRequestRequest request)
        {
            TryValidateRegisterRequest(request);
            var createActivityRequest = new CreateActivityRequest();
            var account = _accountRepository.GetById(accountId)!;
            if (account.Role == BusinessObject.Enums.Role.OrganizationManager)
            {
                var organizationManager = _organizationManagerRepository.GetByAccountID(accountId)!;
                var activity = new Activity
                {
                    ActivityId = Guid.NewGuid(),
                    ProcessingPhaseId = request.ProcessingPhaseId,
                    Content = request.Content,
                    Title = request.Title,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    IsActive = false,
                };



                var activityAdded = _activityRepository.Save(activity);
                if (activityAdded != null)
                {
                    createActivityRequest = new CreateActivityRequest
                    {
                        CreateActivityRequestID = Guid.NewGuid(),
                        ActivityID = activityAdded.ActivityId,
                        CreateByOM = organizationManager.OrganizationManagerID,
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsApproved = false,
                        IsLocked = false,
                        IsPending = true,
                        IsRejected = false,
                    };
                    var notification = new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = account!.AccountID,
                        Content = "Yêu cầu tạo hoạt động của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    };


                    if (request.ActivityImages != null && activityAdded != null)
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

                    var createActivityRequested = _repository.Save(createActivityRequest);
                    if (createActivityRequested != null) _notificationRepository.Save(notification);
                    return createActivityRequested;
                }
                return null;

            }
            else if (account.Role == BusinessObject.Enums.Role.Volunteer)
            {
                var volunteer = _memberRepository.GetByAccountId(accountId)!;
                var activity = new Activity
                {
                    ActivityId = Guid.NewGuid(),
                    ProcessingPhaseId = request.ProcessingPhaseId,
                    Content = request.Content,
                    Title = request.Title,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    IsActive = false,
                };


                var activityAdded = _activityRepository.Save(activity);
                if (activityAdded != null)
                {
                    createActivityRequest = new CreateActivityRequest
                    {
                        CreateActivityRequestID = Guid.NewGuid(),
                        ActivityID = activityAdded.ActivityId,
                        CreateByMember = volunteer.MemberID,
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsApproved = false,
                        IsLocked = false,
                        IsPending = true,
                        IsRejected = false,
                    };
                    var notification = new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = account!.AccountID,
                        Content = "Yêu cầu tạo hoạt động của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    };

                    if (request.ActivityImages != null && activityAdded != null)
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

                    var createActivityRequested = _repository.Save(createActivityRequest);
                    if (createActivityRequested != null) _notificationRepository.Save(notification);
                    return createActivityRequested;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public void AcceptOrRejectCreateActivityRequest(UpdateCreateActivityRequest request)
        {
            var createActivityRequest = _repository.GetById(request.CreateActivityRequestId);
            var moderator = _moderatorRepository.GetById(request.ModeratorId);
            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };
            var activity = new Activity();
            if (createActivityRequest != null && moderator != null)
            {
                if (request.IsAccept == true)
                {

                    activity = _activityRepository.GetById(createActivityRequest.ActivityID)!;
                    activity.IsActive = true;

                    createActivityRequest.IsApproved = true;
                    createActivityRequest.IsPending = false;
                    createActivityRequest.IsRejected = false;
                    createActivityRequest.IsLocked = true;
                    createActivityRequest.ApprovedBy = moderator.ModeratorID;
                    createActivityRequest.ModifiedBy = moderator.AccountID;
                    createActivityRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    if (createActivityRequest.CreateByMember != null)
                    {
                        var member = _memberRepository.GetById((Guid)createActivityRequest.CreateByMember);
                        notification.AccountID = member!.AccountID;
                        notification.Content = "Yêu cầu tạo hoạt động của bạn vừa được duyệt! Vui lòng theo dõi thông tin hoạt động đang diễn ra!";
                    }
                    else
                    {
                        var om = _organizationManagerRepository.GetById((Guid)createActivityRequest.CreateByOM!);
                        notification.AccountID = om!.AccountID;
                        notification.Content = "Yêu cầu tạo hoạt động của bạn vừa được duyệt! Vui lòng theo dõi thông tin hoạt động đang diễn ra!";
                    }

                    _repository.Update(createActivityRequest);
                    _activityRepository.Update(activity);
                    _notificationRepository.Save(notification);

                }
                else
                {
                    activity = _activityRepository.GetById(createActivityRequest.ActivityID)!;
                    activity.IsActive = false;

                    createActivityRequest.IsApproved = false;
                    createActivityRequest.IsPending = false;
                    createActivityRequest.IsRejected = true;
                    createActivityRequest.IsLocked = false;
                    createActivityRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createActivityRequest.ModifiedBy = moderator.AccountID;

                    if (createActivityRequest.CreateByMember != null)
                    {
                        var member = _memberRepository.GetById((Guid)createActivityRequest.CreateByMember);
                        notification.AccountID = member!.AccountID;
                        notification.Content = "Yêu cầu tạo hoạt động của bạn chưa được chấp thuận! Vui lòng cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
                    }
                    else
                    {
                        var om = _organizationManagerRepository.GetById((Guid)createActivityRequest.CreateByOM!);
                        notification.AccountID = om!.AccountID;
                        notification.Content = "Yêu cầu tạo hoạt động của bạn chưa được chấp thuận! Vui lòng cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
                    }

                    _repository.Update(createActivityRequest);
                    _activityRepository.Update(activity);
                    _notificationRepository.Save(notification);
                }

            }
        }



        private void TryValidateRegisterRequest(CreateActivityRequestRequest request)
        {
            if (_phaseRepository.GetById(request.ProcessingPhaseId) == null)
            {
                throw new Exception("Processing phase not found!");
            }

            if (_accountRepository.GetById(request.AccountId) == null)
            {
                throw new Exception("Account not found!");
            }
        }
    }
}
