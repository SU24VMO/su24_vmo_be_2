﻿using BusinessObject.Enums;
using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Request;
using SU24_VMO_API_2.DTOs.Response;
using SU24_VMO_API_2.DTOs.Response.PayosReponse;

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
        private readonly IStatementPhaseRepository _statementPhaseRepository;
        private readonly IDonatePhaseRepository _donatePhaseRepository;
        private readonly IProcessingPhaseStatementFileRepository _processingPhaseStatementFileRepository;
        private readonly IActivityStatementFileRepository _activityStatementFileRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly FirebaseService _firebaseService;

        public CreateActivityRequestService(ICreateActivityRequestRepository repository, IProcessingPhaseRepository phaseRepository,
            IAccountRepository accountRepository, IOrganizationManagerRepository organizationManagerRepository, IActivityRepository activityRepository,
            IMemberRepository memberRepository, INotificationRepository notificationRepository,
            IModeratorRepository moderatorRepository, IActivityImageRepository activityImageRepository, FirebaseService firebaseService,
            IStatementPhaseRepository statementPhaseRepository, IProcessingPhaseStatementFileRepository processingPhaseStatementFileRepository,
            ICampaignRepository campaignRepository, IDonatePhaseRepository donatePhaseRepository, IActivityStatementFileRepository activityStatementFileRepository,
            ITransactionRepository transactionRepository)
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
            _statementPhaseRepository = statementPhaseRepository;
            _processingPhaseStatementFileRepository = processingPhaseStatementFileRepository;
            _campaignRepository = campaignRepository;
            _donatePhaseRepository = donatePhaseRepository;
            _activityStatementFileRepository = activityStatementFileRepository;
            _transactionRepository = transactionRepository;
        }

        public IEnumerable<CreateActivityRequest> GetAll()
        {
            var requests = _repository.GetAll();
            foreach (var request in requests)
            {
                if (request.Moderator != null)
                {
                    request.Moderator.CreateActivityRequests = null;
                    request.Moderator.CreateVolunteerRequests = null;
                    request.Moderator.CreatePostRequests = null;
                    request.Moderator.CreateCampaignRequests = null;
                    request.Moderator.CreateOrganizationManagerRequests = null;
                    request.Moderator.CreateOrganizationRequests = null;

                    if (request.Moderator != null)
                    {
                        request.Moderator.Account = _accountRepository.GetById(request.Moderator.AccountID);
                        if (request.Moderator.Account != null)
                        {
                            request.Moderator.Account.BankingAccounts = null;
                            request.Moderator.Account.Notifications = null;
                            request.Moderator.Account.AccountTokens = null;
                            request.Moderator.Account.Transactions = null;
                        }
                    }
                }

                if (request.OrganizationManager != null)
                {
                    request.OrganizationManager.CreateActivityRequests = null;
                    request.OrganizationManager.CreateCampaignRequests = null;
                    request.OrganizationManager.CreateOrganizationRequests = null;
                    request.OrganizationManager.CreatePostRequests = null;

                }

                if (request.Member != null)
                {
                    request.Member.CreateActivityRequests = null;
                    request.Member.CreateMemberVerifiedRequests = null;
                    request.Member.CreatePostRequests = null;
                    request.Member.CreateCampaignRequests = null;

                }

                if (request.Activity != null)
                {
                    request.Activity.ActivityImages = _activityImageRepository.GetAllActivityImagesByActivityId(request.ActivityID).ToList();
                }
            }
            return requests;
        }

        public CreateActivityRequestOfCampaignTierI? GetAllCreateActivityRequestsOfCampaignTierI(int? pageSize, int? pageNo)
        {
            var listRequestTierI = new List<CreateActivityRequest>();
            var requests = _repository.GetActivitiesRequestOfCampaignTierI(pageSize, pageNo);
            foreach (var request in requests)
            {
                var activity = request.Activity;
                var processingPhase = _phaseRepository.GetById(activity!.ProcessingPhaseId);
                var campaign = _campaignRepository.GetById(processingPhase!.CampaignId);
                if (campaign!.CampaignTier == CampaignTier.FullDisbursementCampaign)
                {
                    if (request.Activity != null)
                    {
                        if (request.Activity.ActivityImages != null)
                        {
                            foreach (var activityImage in request.Activity.ActivityImages)
                            {
                                activityImage.Activity = null;
                            }
                        }
                        else
                        {
                            var activityImages = _activityImageRepository.GetAllActivityImagesByActivityId(request.ActivityID);
                            request.Activity.ActivityImages = activityImages.ToList();
                            foreach (var activityImage in request.Activity.ActivityImages)
                            {
                                activityImage.Activity = null;
                            }
                        }
                    }
                    listRequestTierI.Add(request);
                }
            }
            return new CreateActivityRequestOfCampaignTierI
            {
                CreateActivityRequests = listRequestTierI,
                TotalItem = _repository.GetActivitiesRequestOfCampaignTierI().Count()
            };
        }

        public CreateActivityRequestOfCampaignTierII? GetAllCreateActivityRequestsOfCampaignTierII(int? pageSize, int? pageNo)
        {
            var listRequestTierII = new List<CreateActivityRequest>();
            var requests = _repository.GetActivitiesRequestOfCampaignTierII(pageSize, pageNo);
            foreach (var request in requests)
            {
                var activity = request.Activity;
                var processingPhase = _phaseRepository.GetById(activity!.ProcessingPhaseId);
                var campaign = _campaignRepository.GetById(processingPhase!.CampaignId);
                if (campaign!.CampaignTier == CampaignTier.PartialDisbursementCampaign)
                {
                    if (request.Activity != null)
                    {
                        request.Activity.ProcessingPhase = processingPhase;
                        request.Activity.ProcessingPhase.Activities = null;
                        request.Activity.ProcessingPhase.Campaign = null;
                        if (request.Activity.ActivityImages != null)
                        {
                            var activityImages = _activityImageRepository.GetAllActivityImagesByActivityId(request.ActivityID);
                            request.Activity.ActivityImages = activityImages.ToList();
                            foreach (var activityImage in request.Activity.ActivityImages)
                            {
                                activityImage.Activity = null;
                            }
                        }
                        else
                        {
                            var activityImages = _activityImageRepository.GetAllActivityImagesByActivityId(request.ActivityID);
                            request.Activity.ActivityImages = activityImages.ToList();
                            foreach (var activityImage in request.Activity.ActivityImages)
                            {
                                activityImage.Activity = null;
                            }
                        }

                        request.Activity.ActivityStatementFiles = (_activityStatementFileRepository
                            .GetByActivityId(request.ActivityID) ?? Array.Empty<ActivityStatementFile>()).ToList();
                    }
                    listRequestTierII.Add(request);
                }
            }
            return new CreateActivityRequestOfCampaignTierII
            {
                CreateActivityRequests = listRequestTierII,
                TotalItem = _repository.GetActivitiesRequestOfCampaignTierII().Count()
            };
        }

        public IEnumerable<CreateActivityRequest> GetAllByActivityTitle(string? activityTitle)
        {
            if (!String.IsNullOrEmpty(activityTitle))
            {
                return GetAll().Where(m => m.Activity.Title.Trim().ToLower().Contains(activityTitle.ToLower().Trim()));
            }
            else return GetAll();
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

        //public async Task<CreateActivityRequest?> CreateActivityRequestAsync(Guid accountId, CreateActivityRequestRequest request)
        //{
        //    TryValidateRegisterRequest(request);
        //    var createActivityRequest = new CreateActivityRequest();
        //    var account = _accountRepository.GetById(accountId)!;
        //    if (account.Role == BusinessObject.Enums.Role.OrganizationManager)
        //    {
        //        var organizationManager = _organizationManagerRepository.GetByAccountID(accountId)!;
        //        var activity = new Activity
        //        {
        //            ActivityId = Guid.NewGuid(),
        //            ProcessingPhaseId = request.ProcessingPhaseId,
        //            Content = request.Content,
        //            Title = request.Title,
        //            IsDisable = false,
        //            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //            IsActive = false,
        //        };



        //        var activityAdded = _activityRepository.Save(activity);
        //        if (activityAdded != null)
        //        {
        //            createActivityRequest = new CreateActivityRequest
        //            {
        //                CreateActivityRequestID = Guid.NewGuid(),
        //                ActivityID = activityAdded.ActivityId,
        //                CreateByOM = organizationManager.OrganizationManagerID,
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsApproved = false,
        //                IsLocked = false,
        //                IsPending = true,
        //                IsRejected = false,
        //            };
        //            var notification = new Notification
        //            {
        //                NotificationID = Guid.NewGuid(),
        //                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
        //                AccountID = account!.AccountID,
        //                Content = "Yêu cầu tạo hoạt động của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsSeen = false,
        //            };


        //            if (request.ActivityImages != null)
        //                foreach (var item in request.ActivityImages)
        //                {
        //                    var activityImage = new ActivityImage
        //                    {
        //                        ActivityImageId = Guid.NewGuid(),
        //                        ActivityId = activity.ActivityId,
        //                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                        Link = await _firebaseService.UploadImage(item),
        //                        IsActive = true,
        //                    };
        //                    _activityImageRepository.Save(activityImage);
        //                }

        //            var createActivityRequested = _repository.Save(createActivityRequest);
        //            if (createActivityRequested != null) _notificationRepository.Save(notification);
        //            return createActivityRequested;
        //        }
        //        return null;

        //    }
        //    else if (account.Role == BusinessObject.Enums.Role.Volunteer)
        //    {
        //        var volunteer = _memberRepository.GetByAccountId(accountId)!;
        //        var activity = new Activity
        //        {
        //            ActivityId = Guid.NewGuid(),
        //            ProcessingPhaseId = request.ProcessingPhaseId,
        //            Content = request.Content,
        //            Title = request.Title,
        //            IsDisable = false,
        //            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //            IsActive = false,
        //        };


        //        var activityAdded = _activityRepository.Save(activity);
        //        if (activityAdded != null)
        //        {
        //            createActivityRequest = new CreateActivityRequest
        //            {
        //                CreateActivityRequestID = Guid.NewGuid(),
        //                ActivityID = activityAdded.ActivityId,
        //                CreateByMember = volunteer.MemberID,
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsApproved = false,
        //                IsLocked = false,
        //                IsPending = true,
        //                IsRejected = false,
        //            };
        //            var notification = new Notification
        //            {
        //                NotificationID = Guid.NewGuid(),
        //                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
        //                AccountID = account!.AccountID,
        //                Content = "Yêu cầu tạo hoạt động của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsSeen = false,
        //            };

        //            if (request.ActivityImages != null && activityAdded != null)
        //                foreach (var item in request.ActivityImages)
        //                {
        //                    var activityImage = new ActivityImage
        //                    {
        //                        ActivityImageId = Guid.NewGuid(),
        //                        ActivityId = activity.ActivityId,
        //                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                        Link = await _firebaseService.UploadImage(item),
        //                        IsActive = true,
        //                    };
        //                    _activityImageRepository.Save(activityImage);
        //                }

        //            var createActivityRequested = _repository.Save(createActivityRequest);
        //            if (createActivityRequested != null) _notificationRepository.Save(notification);
        //            return createActivityRequested;
        //        }
        //        return null;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public async Task<CreateActivityRequest?> CreateActivityRequestAsync(Guid accountId, CreateActivityRequestRequest request)
        {
            TryValidateRegisterRequest(request);

            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return null;
            }

            Activity activity = new Activity
            {
                ActivityId = Guid.NewGuid(),
                ProcessingPhaseId = request.ProcessingPhaseId,
                Content = request.Content,
                Title = request.Title,
                IsDisable = false,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsActive = false,
            };

            var activityAdded = _activityRepository.Save(activity);
            if (activityAdded == null)
            {
                return null;
            }

            CreateActivityRequest createActivityRequest;
            Notification notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account!.AccountID,
                Content = "Yêu cầu tạo hoạt động của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };

            if (account.Role == BusinessObject.Enums.Role.OrganizationManager)
            {
                var organizationManager = await _organizationManagerRepository.GetByAccountIDAsync(accountId);
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
            }
            else if (account.Role == BusinessObject.Enums.Role.Volunteer)
            {
                var volunteer = await _memberRepository.GetByAccountIdAsync(accountId);
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
            }
            else
            {
                return null;
            }

            var saveTasks = new List<Task>();
            if (request.ActivityImages != null)
            {
                var uploadTasks = request.ActivityImages.Select(async item =>
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
                });
                saveTasks.AddRange(uploadTasks);
            }

            var createActivityTask = Task.Run(() => _repository.Save(createActivityRequest));
            var saveNotificationTask = Task.Run(() => _notificationRepository.Save(notification));

            saveTasks.Add(createActivityTask);
            saveTasks.Add(saveNotificationTask);

            await Task.WhenAll(saveTasks);

            return await createActivityTask;
        }


        //public async Task<CreateActivityRequest?> CreateActivityTierIIRequestAsync(Guid accountId, CreateActivityTierIIRequestRequest request)
        //{
        //    TryValidateRegisterRequestForTierII(request);
        //    var createActivityRequest = new CreateActivityRequest();
        //    var account = _accountRepository.GetById(accountId)!;
        //    if (account.Role == BusinessObject.Enums.Role.OrganizationManager)
        //    {
        //        var organizationManager = _organizationManagerRepository.GetByAccountID(accountId)!;
        //        var activity = new Activity
        //        {
        //            ActivityId = Guid.NewGuid(),
        //            ProcessingPhaseId = request.ProcessingPhaseId,
        //            Content = request.Content,
        //            Title = request.Title,
        //            IsDisable = false,
        //            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //            IsActive = false,
        //        };



        //        var activityAdded = _activityRepository.Save(activity);
        //        if (activityAdded != null)
        //        {
        //            createActivityRequest = new CreateActivityRequest
        //            {
        //                CreateActivityRequestID = Guid.NewGuid(),
        //                ActivityID = activityAdded.ActivityId,
        //                CreateByOM = organizationManager.OrganizationManagerID,
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsApproved = false,
        //                IsLocked = false,
        //                IsPending = true,
        //                IsRejected = false,
        //            };
        //            var notification = new Notification
        //            {
        //                NotificationID = Guid.NewGuid(),
        //                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
        //                AccountID = account!.AccountID,
        //                Content = "Yêu cầu tạo hoạt động của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsSeen = false,
        //            };


        //            if (request.ActivityImages != null)
        //                foreach (var item in request.ActivityImages)
        //                {
        //                    var activityImage = new ActivityImage
        //                    {
        //                        ActivityImageId = Guid.NewGuid(),
        //                        ActivityId = activity.ActivityId,
        //                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                        Link = await _firebaseService.UploadImage(item),
        //                        IsActive = true,
        //                    };
        //                    _activityImageRepository.Save(activityImage);
        //                }



        //            if (request.ProcessingPhaseStatementFiles != null)
        //            {
        //                foreach (var item in request.ProcessingPhaseStatementFiles)
        //                {
        //                    var statementFile = new ProcessingPhaseStatementFile()
        //                    {
        //                        ProcessingPhaseStatementFileId = Guid.NewGuid(),
        //                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                        Link = await _firebaseService.UploadImage(item),
        //                        ProcessingPhaseId = request.ProcessingPhaseId,
        //                        CreateBy = accountId
        //                    };

        //                    _processingPhaseStatementFileRepository.Save(statementFile);

        //                }
        //            }

        //            var createActivityRequested = _repository.Save(createActivityRequest);
        //            if (createActivityRequested != null) _notificationRepository.Save(notification);
        //            return createActivityRequested;
        //        }
        //        return null;

        //    }
        //    else if (account.Role == BusinessObject.Enums.Role.Volunteer)
        //    {
        //        var volunteer = _memberRepository.GetByAccountId(accountId)!;
        //        var activity = new Activity
        //        {
        //            ActivityId = Guid.NewGuid(),
        //            ProcessingPhaseId = request.ProcessingPhaseId,
        //            Content = request.Content,
        //            Title = request.Title,
        //            IsDisable = false,
        //            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //            IsActive = false,
        //        };


        //        var activityAdded = _activityRepository.Save(activity);
        //        if (activityAdded != null)
        //        {
        //            createActivityRequest = new CreateActivityRequest
        //            {
        //                CreateActivityRequestID = Guid.NewGuid(),
        //                ActivityID = activityAdded.ActivityId,
        //                CreateByMember = volunteer.MemberID,
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsApproved = false,
        //                IsLocked = false,
        //                IsPending = true,
        //                IsRejected = false,
        //            };
        //            var notification = new Notification
        //            {
        //                NotificationID = Guid.NewGuid(),
        //                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
        //                AccountID = account!.AccountID,
        //                Content = "Yêu cầu tạo hoạt động của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsSeen = false,
        //            };

        //            if (request.ActivityImages != null && activityAdded != null)
        //                foreach (var item in request.ActivityImages)
        //                {
        //                    var activityImage = new ActivityImage
        //                    {
        //                        ActivityImageId = Guid.NewGuid(),
        //                        ActivityId = activity.ActivityId,
        //                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                        Link = await _firebaseService.UploadImage(item),
        //                        IsActive = true,
        //                    };
        //                    _activityImageRepository.Save(activityImage);
        //                }


        //            if (request.ProcessingPhaseStatementFiles != null)
        //            {
        //                foreach (var item in request.ProcessingPhaseStatementFiles)
        //                {
        //                    var statementFile = new ProcessingPhaseStatementFile()
        //                    {
        //                        ProcessingPhaseStatementFileId = Guid.NewGuid(),
        //                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                        Link = await _firebaseService.UploadImage(item),
        //                        ProcessingPhaseId = request.ProcessingPhaseId,
        //                        CreateBy = accountId
        //                    };

        //                    _processingPhaseStatementFileRepository.Save(statementFile);

        //                }
        //            }

        //            var createActivityRequested = _repository.Save(createActivityRequest);
        //            if (createActivityRequested != null) _notificationRepository.Save(notification);
        //            return createActivityRequested;
        //        }
        //        return null;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        public async Task<CreateActivityRequest?> CreateActivityTierIIRequestAsync(Guid accountId, CreateActivityTierIIRequestRequest request)
        {
            TryValidateRegisterRequestForTierII(request);

            var transactions =
                _transactionRepository.GetTransactionsByProcessingPhaseIdWithTypeIsTransfer(request.ProcessingPhaseId);
            if (transactions == null || !transactions.Any())
            {
                throw new BadRequestException(
                    "Hiện hệ thống chưa giải ngân cho tiến trình này, vì vậy chưa thể đăng hoạt động!");
            }

            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return null;
            }

            Activity activity = new Activity
            {
                ActivityId = Guid.NewGuid(),
                ProcessingPhaseId = request.ProcessingPhaseId,
                Content = request.Content,
                Title = request.Title,
                IsDisable = false,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsActive = false,
            };

            var activityAdded = _activityRepository.Save(activity);
            if (activityAdded == null)
            {
                return null;
            }

            CreateActivityRequest createActivityRequest;
            Notification notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account.AccountID,
                Content = "Yêu cầu tạo hoạt động của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };

            if (account.Role == BusinessObject.Enums.Role.OrganizationManager)
            {
                var organizationManager = await _organizationManagerRepository.GetByAccountIDAsync(accountId);
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
            }
            else if (account.Role == BusinessObject.Enums.Role.Volunteer)
            {
                var volunteer = await _memberRepository.GetByAccountIdAsync(accountId);
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
            }
            else
            {
                return null;
            }

            var saveTasks = new List<Task>();

            if (request.ActivityImages != null)
            {
                var uploadImageTasks = request.ActivityImages.Select(async item =>
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
                });
                saveTasks.AddRange(uploadImageTasks);
            }

            if (request.ProcessingPhaseStatementFiles != null)
            {
                var uploadFileTasks = request.ProcessingPhaseStatementFiles.Select(async item =>
                {
                    var statementFile = new ProcessingPhaseStatementFile
                    {
                        ProcessingPhaseStatementFileId = Guid.NewGuid(),
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        Link = await _firebaseService.UploadImage(item),
                        ProcessingPhaseId = request.ProcessingPhaseId,
                        CreateBy = accountId,
                    };
                    _processingPhaseStatementFileRepository.Save(statementFile);

                    var activityStatementFile = new ActivityStatementFile
                    {
                        ActivityStatementFileId = Guid.NewGuid(),
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        Link = await _firebaseService.UploadImage(item),
                        ActivityId = activity.ActivityId,
                        Description = request.Content,
                        IsActive = true,
                    };
                    _activityStatementFileRepository.Save(activityStatementFile);
                });
                saveTasks.AddRange(uploadFileTasks);
            }

            var createActivityTask = Task.Run(() => _repository.Save(createActivityRequest));
            var saveNotificationTask = Task.Run(() => _notificationRepository.Save(notification));

            saveTasks.Add(createActivityTask);
            saveTasks.Add(saveNotificationTask);

            await Task.WhenAll(saveTasks);

            return await createActivityTask;
        }


        public async Task<bool> UpdateCreateActivityRequestRequest(Guid createActivityRequestId, UpdateCreateActivityRequestRequest updateRequest)
        {
            var requestExisted = _repository.GetById(createActivityRequestId);
            if (requestExisted == null)
            {
                throw new NotFoundException("Đơn tạo yêu cầu cho hoạt động này không tồn tại!");
            }

            var activityExisted = _activityRepository.GetById(requestExisted.ActivityID);
            if (activityExisted == null)
            {
                throw new NotFoundException("Không tìm thấy hoạt động này!");
            }

            if (requestExisted.IsApproved)
            {
                throw new BadRequestException("Đơn tạo hoạt động này hiện đã được duyệt, vì vậy mọi thông tin về đơn này hiện không thể chỉnh sửa!");
            }

            if (!String.IsNullOrEmpty(updateRequest.Content))
            {
                activityExisted.Content = updateRequest.Content;
            }

            if (!String.IsNullOrEmpty(updateRequest.Title))
            {
                activityExisted.Title = updateRequest.Title;
            }

            if (updateRequest.ActivityImages != null)
            {
                var activityImagesExisted =
                    _activityImageRepository.GetAllActivityImagesByActivityId(activityExisted.ActivityId);
                foreach (var imageExisted in activityImagesExisted)
                {
                    _activityImageRepository.DeleteById(imageExisted.ActivityImageId);
                }

                foreach (var updateRequestImage in updateRequest.ActivityImages)
                {
                    var activityImage = new ActivityImage
                    {
                        ActivityImageId = Guid.NewGuid(),
                        ActivityId = activityExisted.ActivityId,
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        Link = await _firebaseService.UploadImage(updateRequestImage),
                        IsActive = true,
                    };
                    _activityImageRepository.Save(activityImage);
                }
            }
            _activityRepository.Update(activityExisted);
            return true;
        }



        public async Task<bool> UpdateCreateActivityRequestCampaignTierIIRequest(Guid createActivityRequestId, UpdateCreateActivityRequestRequest updateRequest)
        {
            var requestExisted = _repository.GetById(createActivityRequestId);
            if (requestExisted == null)
            {
                throw new NotFoundException("Đơn tạo yêu cầu cho hoạt động này không tồn tại!");
            }

            var activityExisted = _activityRepository.GetById(requestExisted.ActivityID);
            if (activityExisted == null)
            {
                throw new NotFoundException("Không tìm thấy hoạt động này!");
            }

            if (requestExisted.IsApproved)
            {
                throw new BadRequestException("Đơn tạo hoạt động này hiện đã được duyệt, vì vậy mọi thông tin về đơn này hiện không thể chỉnh sửa!");
            }

            if (!String.IsNullOrEmpty(updateRequest.Content))
            {
                activityExisted.Content = updateRequest.Content;
            }

            if (!String.IsNullOrEmpty(updateRequest.Title))
            {
                activityExisted.Title = updateRequest.Title;
            }

            if (updateRequest.ActivityImages != null)
            {
                var activityImagesExisted =
                    _activityImageRepository.GetAllActivityImagesByActivityId(activityExisted.ActivityId);
                foreach (var imageExisted in activityImagesExisted)
                {
                    _activityImageRepository.DeleteById(imageExisted.ActivityImageId);
                }

                foreach (var updateRequestImage in updateRequest.ActivityImages)
                {
                    var activityImage = new ActivityImage
                    {
                        ActivityImageId = Guid.NewGuid(),
                        ActivityId = activityExisted.ActivityId,
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        Link = await _firebaseService.UploadImage(updateRequestImage),
                        IsActive = true,
                    };
                    _activityImageRepository.Save(activityImage);
                }


                var processingPhaseStatementFilesExisted =
                    _processingPhaseStatementFileRepository.GetProcessingPhaseStatementFilesByProcessingPhaseId(
                        activityExisted.ProcessingPhaseId);

                foreach (var processingPhase in processingPhaseStatementFilesExisted)
                {
                    _activityImageRepository.DeleteById(processingPhase.ProcessingPhaseStatementFileId);
                }

                foreach (var updateRequestImage in updateRequest.ProcessingPhaseStatementFiles)
                {
                    var statementFileImage = new ProcessingPhaseStatementFile()
                    {
                        ProcessingPhaseStatementFileId = Guid.NewGuid(),
                        ProcessingPhaseId = activityExisted.ProcessingPhaseId,
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        Link = await _firebaseService.UploadImage(updateRequestImage),
                    };
                    _processingPhaseStatementFileRepository.Save(statementFileImage);
                }

            }
            _activityRepository.Update(activityExisted);
            return true;
        }



        public void AcceptOrRejectCreateActivityRequest(UpdateCreateActivityRequest request)
        {
            var createActivityRequest = _repository.GetById(request.CreateActivityRequestId);
            if (createActivityRequest == null) { throw new NotFoundException("Yêu cầu không tìm thấy!"); }
            var moderator = _moderatorRepository.GetById(request.ModeratorId);
            if (moderator == null) { throw new NotFoundException("Không tìm thấy người duyệt yêu cầu!"); }
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
                if (request.IsApproved == true)
                {

                    activity = _activityRepository.GetById(createActivityRequest.ActivityID)!;
                    activity.IsActive = true;
                    //activity.IsDisable = false;

                    createActivityRequest.IsApproved = true;
                    createActivityRequest.IsPending = false;
                    createActivityRequest.IsRejected = false;
                    createActivityRequest.IsLocked = true;
                    createActivityRequest.ApprovedBy = moderator.ModeratorID;
                    createActivityRequest.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
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
                    //activity.IsDisable = true;

                    createActivityRequest.IsApproved = false;
                    createActivityRequest.IsPending = false;
                    createActivityRequest.IsRejected = true;
                    createActivityRequest.IsLocked = false;
                    createActivityRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createActivityRequest.ApprovedBy = moderator.ModeratorID;
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


        public void AcceptOrRejectCreateActivityRequestTierII(UpdateCreateActivityRequest request)
        {
            var createActivityRequest = _repository.GetById(request.CreateActivityRequestId);
            if (createActivityRequest == null) { throw new NotFoundException("Yêu cầu không tìm thấy!"); }
            var moderator = _moderatorRepository.GetById(request.ModeratorId);
            if (moderator == null) { throw new NotFoundException("Không tìm thấy người duyệt yêu cầu!"); }
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
                if (request.IsApproved == true)
                {

                    activity = _activityRepository.GetById(createActivityRequest.ActivityID)!;
                    activity.IsActive = true;
                    activity.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);

                    //var processingPhase = _phaseRepository.GetById(activity.ProcessingPhaseId)!;
                    //var campaign = _campaignRepository.GetById(processingPhase.CampaignId)!;
                    //var maxPriority = _phaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID).Max(p => p.Priority);
                    //var finalProcessingPhase = _phaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID)
                    //    .FirstOrDefault(p => p.Priority == maxPriority);

                    //if (processingPhase.Priority != maxPriority)
                    //{
                    //    processingPhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);
                    //    processingPhase.IsProcessing = false;
                    //    processingPhase.IsActive = true;
                    //    processingPhase.IsEnd = true;
                    //    processingPhase.IsLocked = true;
                    //    _phaseRepository.Update(processingPhase);

                    //    var nextProcessingPhase = _phaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID).FirstOrDefault(p => p.Priority == processingPhase.Priority + 1);
                    //    if (nextProcessingPhase != null)
                    //    {
                    //        nextProcessingPhase.StartDate = TimeHelper.GetTime(DateTime.UtcNow);
                    //        nextProcessingPhase.IsProcessing = true;
                    //        nextProcessingPhase.IsActive = true;
                    //        nextProcessingPhase.IsLocked = false;
                    //        nextProcessingPhase.IsEnd = false;
                    //        _phaseRepository.Update(nextProcessingPhase);
                    //    }
                    //}
                    //else
                    //{
                    //    processingPhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);
                    //    processingPhase.IsProcessing = false;
                    //    processingPhase.IsActive = true;
                    //    processingPhase.IsEnd = true;
                    //    processingPhase.IsLocked = true;
                    //    _phaseRepository.Update(processingPhase);

                    //    campaign.IsComplete = true;
                    //    campaign.ActualEndDate = TimeHelper.GetTime(DateTime.UtcNow);
                    //    var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID);
                    //    if (donatePhase != null)
                    //    {
                    //        campaign.CanBeDonated = false;
                    //        donatePhase.IsEnd = true;
                    //        donatePhase.IsProcessing = false;
                    //        donatePhase.IsLocked = true;
                    //        donatePhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);
                    //    }
                    //    _campaignRepository.Update(campaign);
                    //}

                    //activity.IsDisable = false;

                    createActivityRequest.IsApproved = true;
                    createActivityRequest.IsPending = false;
                    createActivityRequest.IsRejected = false;
                    createActivityRequest.IsLocked = true;
                    createActivityRequest.ApprovedBy = moderator.ModeratorID;
                    createActivityRequest.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
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
                    //activity.IsDisable = true;

                    createActivityRequest.IsApproved = false;
                    createActivityRequest.IsPending = false;
                    createActivityRequest.IsRejected = true;
                    createActivityRequest.IsLocked = false;
                    createActivityRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createActivityRequest.ApprovedBy = moderator.ModeratorID;
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
                throw new Exception("Giai đoạn giải ngân không tìm thấy!");
            }

            if (_accountRepository.GetById(request.AccountId) == null)
            {
                throw new Exception("Tài khoản không tìm thấy!");
            }
        }

        private void TryValidateRegisterRequestForTierII(CreateActivityTierIIRequestRequest request)
        {
            if (_phaseRepository.GetById(request.ProcessingPhaseId) == null)
            {
                throw new Exception("Giai đoạn giải ngân không tìm thấy!");
            }

            if (_accountRepository.GetById(request.AccountId) == null)
            {
                throw new Exception("Tài khoản không tìm thấy!");
            }
        }
    }
}
