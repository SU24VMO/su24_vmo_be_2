﻿using BusinessObject.Models;
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
        private readonly IRequestManagerRepository _requestManagerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationRepository _notificationRepository;

        public CreateActivityRequestService(ICreateActivityRequestRepository repository, IProcessingPhaseRepository phaseRepository, IAccountRepository accountRepository,
            IOrganizationManagerRepository organizationManagerRepository, IActivityRepository activityRepository, IUserRepository userRepository, INotificationRepository notificationRepository, 
            IRequestManagerRepository requestManagerRepository)
        {
            _repository = repository;
            _phaseRepository = phaseRepository;
            _accountRepository = accountRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _activityRepository = activityRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _requestManagerRepository = requestManagerRepository;
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

        public CreateActivityRequest? CreateActivityRequest(Guid accountId, CreateActivityRequestRequest request)
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
                    IsActive = true,
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
                    var createActivityRequested = _repository.Save(createActivityRequest);
                    if (createActivityRequested != null) _notificationRepository.Save(notification);
                    return createActivityRequested;
                }
                return null;

            }
            else if (account.Role == BusinessObject.Enums.Role.Member)
            {
                var member = _userRepository.GetByAccountId(accountId)!;
                var activity = new Activity
                {
                    ActivityId = Guid.NewGuid(),
                    ProcessingPhaseId = request.ProcessingPhaseId,
                    Content = request.Content,
                    Title = request.Title,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    IsActive = true,
                };


                var activityAdded = _activityRepository.Save(activity);
                if (activityAdded != null)
                {
                    createActivityRequest = new CreateActivityRequest
                    {
                        CreateActivityRequestID = Guid.NewGuid(),
                        ActivityID = activityAdded.ActivityId,
                        CreateByOM = member.UserID,
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
            var requestManager = _requestManagerRepository.GetById(request.RequestManagerId);
            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };
            if (createActivityRequest != null && requestManager != null)
            {
                if (request.IsAccept == true)
                {
                    createActivityRequest.IsApproved = true;
                    createActivityRequest.IsPending = false;
                    createActivityRequest.IsRejected = false;
                    createActivityRequest.IsLocked = true;
                    createActivityRequest.ApprovedBy = requestManager.RequestManagerID;
                    createActivityRequest.ModifiedBy = requestManager.AccountID;
                    createActivityRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    if (createActivityRequest.CreateByUser != null)
                    {
                        var user = _userRepository.GetById((Guid)createActivityRequest.CreateByUser);
                        notification.AccountID = user!.AccountID;
                        notification.Content = "Yêu cầu tạo hoạt động của bạn vừa được duyệt! Vui lòng theo dõi thông tin hoạt động đang diễn ra!";
                    }
                    else
                    {
                        var om = _organizationManagerRepository.GetById((Guid)createActivityRequest.CreateByOM!);
                        notification.AccountID = om!.AccountID;
                        notification.Content = "Yêu cầu tạo hoạt động của bạn vừa được duyệt! Vui lòng theo dõi thông tin hoạt động đang diễn ra!";
                    }
                    

                    _repository.Update(createActivityRequest);
                    _notificationRepository.Save(notification);

                }
                else
                {
                    createActivityRequest.IsApproved = false;
                    createActivityRequest.IsPending = false;
                    createActivityRequest.IsRejected = true;
                    createActivityRequest.IsLocked = false;
                    createActivityRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createActivityRequest.ModifiedBy = requestManager.AccountID;

                    if (createActivityRequest.CreateByUser != null)
                    {
                        var user = _userRepository.GetById((Guid)createActivityRequest.CreateByUser);
                        notification.AccountID = user!.AccountID;
                        notification.Content = "Yêu cầu tạo hoạt động của bạn chưa được chấp thuận! Vui lòng cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
                    }
                    else
                    {
                        var om = _organizationManagerRepository.GetById((Guid)createActivityRequest.CreateByOM!);
                        notification.AccountID = om!.AccountID;
                        notification.Content = "Yêu cầu tạo hoạt động của bạn chưa được chấp thuận! Vui lòng cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
                    }
                    _repository.Update(createActivityRequest);
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