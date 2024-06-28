using BusinessObject.Enums;
using BusinessObject.Models;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class CreateCampaignRequestService
    {
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IDonatePhaseRepository _donatePhaseRepository;
        private readonly IProcessingPhaseRepository _processingPhaseRepository;
        private readonly IStatementPhaseRepository _statementPhaseRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly CampaignService _campaignService;
        private readonly FirebaseService _firebaseService;

        public CreateCampaignRequestService(ICreateCampaignRequestRepository createCampaignRequestRepository, FirebaseService firebaseService,
            IUserRepository userRepository, IAccountRepository accountRepository, CampaignService campaignService, INotificationRepository notificationRepository,
            IDonatePhaseRepository donatePhaseRepository, IProcessingPhaseRepository processingPhaseRepository, IStatementPhaseRepository statementPhaseRepository,
            IOrganizationManagerRepository organizationManagerRepository, ICampaignRepository campaignRepository)
        {
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _firebaseService = firebaseService;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _campaignService = campaignService;
            _notificationRepository = notificationRepository;
            _donatePhaseRepository = donatePhaseRepository;
            _processingPhaseRepository = processingPhaseRepository;
            _statementPhaseRepository = statementPhaseRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _campaignRepository = campaignRepository;
        }


        public IEnumerable<CreateCampaignRequest>? GetCreateCampaignRequests()
        {
            var requests = _createCampaignRequestRepository.GetAll();
            foreach (var request in requests)
            {
                if (request.OrganizationManager != null)
                {
                    if (request.OrganizationManager.CreateCampaignRequests != null)
                        request.OrganizationManager.CreateCampaignRequests.Clear();
                    if (request.OrganizationManager.CreatePostRequests != null)
                        request.OrganizationManager.CreatePostRequests.Clear();
                    if (request.OrganizationManager.CreateOrganizationRequests != null)
                        request.OrganizationManager.CreateOrganizationRequests.Clear();
                }
                if (request.RequestManager != null)
                {
                    if (request.RequestManager.CreateCampaignRequests != null)
                        request.RequestManager.CreateCampaignRequests.Clear();
                    if (request.RequestManager.CreatePostRequests != null)
                        request.RequestManager.CreatePostRequests.Clear();
                    if (request.RequestManager.CreateMemberRequests != null)
                        request.RequestManager.CreateMemberRequests.Clear();
                    if (request.RequestManager.CreateActivityRequests != null)
                        request.RequestManager.CreateActivityRequests.Clear();
                    if (request.RequestManager.CreateOrganizationManagerRequests != null)
                        request.RequestManager.CreateOrganizationManagerRequests.Clear();
                    if (request.RequestManager.CreateOrganizationRequests != null)
                        request.RequestManager.CreateOrganizationRequests.Clear();
                }
                var campaign = _campaignRepository.GetById(request.CampaignID);
                request.Campaign = campaign;
            }
            return requests;
        }

        public async Task<CreateCampaignRequest?> CreateCampaignRequestAsync(Guid accountId, CreateCampaignRequestRequest request)
        {
            TryValidateRegisterRequest(request);

            var account = _accountRepository.GetById(accountId);
            var om = new OrganizationManager();
            var member = new User();
            if (account == null)
            {
                return null;
            }
            else
            {
                if (account.Role == Role.OrganizationManager)
                {
                    om = _organizationManagerRepository.GetByAccountID(accountId);
                    var linkImageCampaign = "";
                    if (request.ImageCampaign != null)
                    {
                        linkImageCampaign = await _firebaseService.UploadImage(request.ImageCampaign);
                    }

                    var linkConfirmForm = "";
                    if (request.ApplicationConfirmForm != null)
                    {
                        linkConfirmForm = await _firebaseService.UploadImage(request.ApplicationConfirmForm);
                    }

                    var campaign = new Campaign
                    {
                        CampaignID = Guid.NewGuid(),
                        OrganizationID = request.OrganizationId,
                        Name = request.Name,
                        CampaignTypeID = request.CampaignTypeId,
                        Address = request.Address,
                        Description = request.Description,
                        Image = linkImageCampaign,
                        StartDate = request.StartDate,
                        ExpectedEndDate = request.ExpectedEndDate,
                        TargetAmount = request.TargetAmount,
                        ApplicationConfirmForm = linkConfirmForm,
                        IsTransparent = false,
                        CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                        CanBeDonated = true,
                        IsActive = false,
                        IsModify = false,
                        IsComplete = false,
                    };

                    var qrImageLink = "";
                    if (request.QRCode != null)
                    {
                        qrImageLink = await _firebaseService.UploadImage(request.QRCode);
                    }
                    var bankingAccount = new BankingAccount
                    {
                        BankingAccountID = Guid.NewGuid(),
                        BankingName = request.BankingName!,
                        AccountNumber = request.BankingAccountNumber!,
                        AccountName = request.AccountName,
                        CreatedAt = TimeHelper.GetTime(DateTime.UtcNow),
                        AccountId = accountId,
                        IsAvailable = true,
                        QRCode = qrImageLink
                    };

                    var user = _userRepository.GetByAccountId(accountId);


                    var createCampaignRequest = new CreateCampaignRequest
                    {
                        CreateCampaignRequestID = Guid.NewGuid(),
                        CampaignID = campaign.CampaignID,
                        Campaign = campaign,
                        CreateByOM = om!.OrganizationManagerID,
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsApproved = false,
                        IsPending = true,
                        IsRejected = false,
                        IsLocked = false,
                    };

                    var notification = new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = account!.AccountID,
                        Content = "Yêu cầu tạo chiến dịch của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    };


                    var createCampaignRequestCreated = _createCampaignRequestRepository.SaveWithBankingAccount(createCampaignRequest, bankingAccount);
                    if (createCampaignRequestCreated != null)
                    {
                        _notificationRepository.Save(notification);
                    }
                    return createCampaignRequestCreated;
                }
                else if (account.Role == Role.Member)
                {
                    member = _userRepository.GetByAccountId(accountId);
                    var linkImageCampaign = "";
                    if (request.ImageCampaign != null)
                    {
                        linkImageCampaign = await _firebaseService.UploadImage(request.ImageCampaign);
                    }

                    var linkConfirmForm = "";
                    if (request.ApplicationConfirmForm != null)
                    {
                        linkConfirmForm = await _firebaseService.UploadImage(request.ApplicationConfirmForm);
                    }

                    var campaign = new Campaign
                    {
                        CampaignID = Guid.NewGuid(),
                        Name = request.Name,
                        CampaignTypeID = request.CampaignTypeId,
                        Address = request.Address,
                        Description = request.Description,
                        Image = linkImageCampaign,
                        StartDate = request.StartDate,
                        ExpectedEndDate = request.ExpectedEndDate,
                        TargetAmount = request.TargetAmount,
                        ApplicationConfirmForm = linkConfirmForm,
                        IsTransparent = false,
                        CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                        CanBeDonated = true,
                        IsActive = false,
                        IsModify = false,
                        IsComplete = false,
                    };

                    var qrImageLink = "";
                    if (request.QRCode != null)
                    {
                        qrImageLink = await _firebaseService.UploadImage(request.QRCode);
                    }
                    var bankingAccount = new BankingAccount
                    {
                        BankingAccountID = Guid.NewGuid(),
                        BankingName = request.BankingName!,
                        AccountNumber = request.BankingAccountNumber!,
                        AccountName = request.AccountName,
                        CreatedAt = TimeHelper.GetTime(DateTime.UtcNow),
                        AccountId = accountId,
                        IsAvailable = true,
                        QRCode = qrImageLink
                    };

                    var createCampaignRequest = new CreateCampaignRequest
                    {
                        CreateCampaignRequestID = Guid.NewGuid(),
                        CampaignID = campaign.CampaignID,
                        Campaign = campaign,
                        CreateByUser = member!.UserID,
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsApproved = false,
                        IsPending = true,
                        IsRejected = false,
                        IsLocked = false,
                    };

                    var notification = new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = account!.AccountID,
                        Content = "Yêu cầu tạo chiến dịch của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    };


                    var createCampaignRequestCreated = _createCampaignRequestRepository.SaveWithBankingAccount(createCampaignRequest, bankingAccount);
                    if (createCampaignRequestCreated != null)
                    {
                        _notificationRepository.Save(notification);
                    }
                    return createCampaignRequestCreated;
                }
                else
                {
                    return null;
                }
            }


        }


        public bool AcceptOrRejectCreateCampaignRequest(UpdateCreateCampaignRequest updateCampaignRequest)
        {
            TryValidateUpdateCreateCampaignRequest(updateCampaignRequest);
            var request = _createCampaignRequestRepository.GetById((Guid)updateCampaignRequest.CreateCampaignRequestID!);
            var result = false;
            var campaign = new Campaign();
            var donatePhase = new DonatePhase();
            var processingPhase = new ProcessingPhase();
            var statementPhase = new StatementPhase();
            var user = new User();
            var om = new OrganizationManager();
            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };
            if (request == null)
            {
                return result;
            }

            if (request.CreateByUser != null)
            {
                if (updateCampaignRequest.IsApproved == true)
                {
                    campaign = _campaignService.GetCampaignByCampaignId(request!.CampaignID)!;
                    campaign.IsActive = true;
                    campaign.IsTransparent = true;

                    request.IsApproved = true;
                    request.IsPending = false;
                    request.IsLocked = false;
                    request.ApprovedBy = updateCampaignRequest.RequestManagerId;
                    request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    request.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                    request.IsRejected = false;
                    result = true;

                    user = _userRepository.GetById((Guid)request!.CreateByUser!);

                    notification.AccountID = user!.AccountID;
                    notification.Content = "Yêu cầu tạo chiến dịch của bạn vừa được duyệt! Vui lòng theo dõi thông tin chiến dịch đang diễn ra!";

                    var donatePhaseExisted = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID);
                    if (donatePhaseExisted == null)
                    {
                        donatePhase = new DonatePhase
                        {
                            DonatePhaseId = Guid.NewGuid(),
                            CampaignId = campaign.CampaignID,
                            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                            CurrentMoney = "0",
                            IsProcessing = true,
                            IsLocked = false,
                            IsEnd = false,
                            Name = "Giai đoạn ủng hộ",
                            Percent = 0,
                            StartDate = TimeHelper.GetTime(DateTime.UtcNow),
                        };
                        _donatePhaseRepository.Save(donatePhase);
                    }

                    var processingPhaseExisted = _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID);
                    if (processingPhaseExisted == null)
                    {
                        processingPhase = new ProcessingPhase
                        {
                            ProcessingPhaseId = Guid.NewGuid(),
                            CampaignId = campaign.CampaignID,
                            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                            IsProcessing = false,
                            IsLocked = false,
                            IsEnd = false,
                            Name = "Giai đoạn xử lý, giải ngân",
                        };
                        _processingPhaseRepository.Save(processingPhase);

                    }

                    var statementPhaseExisted = _statementPhaseRepository.GetStatementPhaseByCampaignId(campaign.CampaignID);
                    if (statementPhaseExisted == null)
                    {
                        statementPhase = new StatementPhase
                        {
                            StatementPhaseId = Guid.NewGuid(),
                            CampaignId = campaign.CampaignID,
                            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                            IsProcessing = false,
                            IsLocked = false,
                            IsEnd = false,
                            Name = "Giai đoạn sao kê",
                        };
                        _statementPhaseRepository.Save(statementPhase);
                    }

                }
                else
                {
                    campaign = _campaignService.GetCampaignByCampaignId(request!.CampaignID)!;
                    campaign.IsActive = false;
                    campaign.IsTransparent = false;

                    request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    request.IsApproved = false;
                    request.IsPending = false;
                    request.IsLocked = false;
                    request.IsRejected = true;
                    result = true;

                    user = _userRepository.GetById((Guid)request!.CreateByUser!);

                    notification.AccountID = user!.AccountID;
                    notification.Content = "Yêu cầu tạo chiến dịch của bạn chưa được chấp thuận! Vui lòng cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
                }
            }
            else
            {
                if (updateCampaignRequest.IsApproved == true)
                {
                    campaign = _campaignService.GetCampaignByCampaignId(request!.CampaignID)!;
                    campaign.IsActive = true;
                    campaign.IsTransparent = true;


                    request.ApprovedBy = updateCampaignRequest.RequestManagerId;
                    request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    request.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                    request.IsApproved = true;
                    request.IsPending = false;
                    request.IsLocked = false;
                    request.IsRejected = false;
                    result = true;

                    om = _organizationManagerRepository.GetById((Guid)request!.CreateByOM!);

                    notification.AccountID = om!.AccountID;
                    notification.Content = "Yêu cầu tạo chiến dịch của bạn vừa được duyệt! Vui lòng theo dõi thông tin chiến dịch đang diễn ra!";

                    var donatePhaseExisted = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID);
                    if (donatePhaseExisted == null)
                    {
                        donatePhase = new DonatePhase
                        {
                            DonatePhaseId = Guid.NewGuid(),
                            CampaignId = campaign.CampaignID,
                            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                            CurrentMoney = "0",
                            IsProcessing = true,
                            IsLocked = false,
                            IsEnd = false,
                            Name = "Giai đoạn ủng hộ",
                            Percent = 0,
                            StartDate = TimeHelper.GetTime(DateTime.UtcNow),
                        };
                        _donatePhaseRepository.Save(donatePhase);
                    }

                    var processingPhaseExisted = _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID);
                    if (processingPhaseExisted == null)
                    {
                        processingPhase = new ProcessingPhase
                        {
                            ProcessingPhaseId = Guid.NewGuid(),
                            CampaignId = campaign.CampaignID,
                            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                            IsProcessing = false,
                            IsLocked = false,
                            IsEnd = false,
                            Name = "Giai đoạn xử lý, giải ngân",
                        };
                        _processingPhaseRepository.Save(processingPhase);

                    }

                    var statementPhaseExisted = _statementPhaseRepository.GetStatementPhaseByCampaignId(campaign.CampaignID);
                    if (statementPhaseExisted == null)
                    {
                        statementPhase = new StatementPhase
                        {
                            StatementPhaseId = Guid.NewGuid(),
                            CampaignId = campaign.CampaignID,
                            CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                            IsProcessing = false,
                            IsLocked = false,
                            IsEnd = false,
                            Name = "Giai đoạn sao kê",
                        };
                        _statementPhaseRepository.Save(statementPhase);
                    }

                }
                else
                {
                    campaign = _campaignService.GetCampaignByCampaignId(request!.CampaignID)!;
                    campaign.IsActive = false;
                    campaign.IsTransparent = false;

                    request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    request.IsApproved = false;
                    request.IsPending = false;
                    request.IsLocked = false;
                    request.IsRejected = true;
                    result = true;

                    om = _organizationManagerRepository.GetById((Guid)request!.CreateByOM!);

                    notification.AccountID = om!.AccountID;
                    notification.Content = "Yêu cầu tạo chiến dịch của bạn chưa được chấp thuận! Vui lòng cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
                }
            }


            _createCampaignRequestRepository.Update(request);
            _campaignService.UpdateCampaign(campaign);
            _notificationRepository.Save(notification);

            return result;
        }


        //public bool AcceptOrRejectCreateCampaignRequestByOMRole(UpdateCreateCampaignRequest updateCampaignRequest)
        //{
        //    TryValidateUpdateCreateCampaignRequest(updateCampaignRequest);
        //    var request = _createCampaignRequestRepository.GetById((Guid)updateCampaignRequest.CreateCampaignRequestID!);
        //    var result = false;
        //    var campaign = new Campaign();
        //    var donatePhase = new DonatePhase();
        //    var processingPhase = new ProcessingPhase();
        //    var statementPhase = new StatementPhase();
        //    if (request == null)
        //    {
        //        return result;
        //    }


        //    var notification = new Notification
        //    {
        //        NotificationID = Guid.NewGuid(),
        //        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
        //        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //        IsSeen = false,
        //    };
        //    if (updateCampaignRequest.IsApproved == true)
        //    {
        //        campaign = _campaignService.GetCampaignByCampaignId(request!.CampaignID)!;
        //        campaign.IsActive = true;
        //        campaign.IsTransparent = true;

        //        request.IsApproved = true;
        //        request.IsPending = false;
        //        request.IsLocked = false;
        //        request.IsRejected = false;
        //        result = true;

        //        var om = _organizationManagerRepository.GetById((Guid)request!.CreateByUser!);

        //        notification.AccountID = om!.AccountID;
        //        notification.Content = "Yêu cầu tạo chiến dịch của bạn vừa được duyệt! Vui lòng theo dõi thông tin chiến dịch đang diễn ra!";

        //        var donatePhaseExisted = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID);
        //        if (donatePhaseExisted == null)
        //        {
        //            donatePhase = new DonatePhase
        //            {
        //                DonatePhaseId = Guid.NewGuid(),
        //                CampaignId = campaign.CampaignID,
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                CurrentMoney = "0",
        //                IsProcessing = true,
        //                IsEnd = false,
        //                Name = "Giai đoạn ủng hộ",
        //                Percent = 0,
        //                StartDate = TimeHelper.GetTime(DateTime.UtcNow),
        //            };
        //            _donatePhaseRepository.Save(donatePhase);
        //        }

        //        var processingPhaseExisted = _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID);
        //        if (processingPhaseExisted == null)
        //        {
        //            processingPhase = new ProcessingPhase
        //            {
        //                ProcessingPhaseId = Guid.NewGuid(),
        //                CampaignId = campaign.CampaignID,
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsProcessing = false,
        //                IsEnd = false,
        //                Name = "Giai đoạn xử lý, giải ngân",
        //            };
        //            _processingPhaseRepository.Save(processingPhase);

        //        }

        //        var statementPhaseExisted = _statementPhaseRepository.GetStatementPhaseByCampaignId(campaign.CampaignID);
        //        if (statementPhaseExisted == null)
        //        {
        //            statementPhase = new StatementPhase
        //            {
        //                StatementPhaseId = Guid.NewGuid(),
        //                CampaignId = campaign.CampaignID,
        //                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
        //                IsProcessing = false,
        //                IsEnd = false,
        //                Name = "Giai đoạn sao kê",
        //            };
        //            _statementPhaseRepository.Save(statementPhase);
        //        }

        //    }
        //    else
        //    {
        //        request.IsApproved = false;
        //        request.IsPending = false;
        //        request.IsLocked = false;
        //        request.IsRejected = true;
        //        result = true;

        //        var om = _organizationManagerRepository.GetById((Guid)request!.CreateByUser!);

        //        notification.AccountID = om!.AccountID;
        //        notification.Content = "Yêu cầu tạo chiến dịch của bạn chưa được chấp thuận! Vui lòng cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
        //    }

        //    _createCampaignRequestRepository.Update(request);
        //    _campaignService.UpdateCampaign(campaign);
        //    _notificationRepository.Save(notification);

        //    return result;
        //}



        private void TryValidateUpdateCreateCampaignRequest(UpdateCreateCampaignRequest request)
        {
            if (String.IsNullOrEmpty(request.CreateCampaignRequestID.ToString()))
            {
                throw new Exception("Id must not be null or empty!");
            }
            if (String.IsNullOrEmpty(request.IsApproved.ToString()))
            {
                throw new Exception("Status must not be null or empty!");
            }
        }


        private void TryValidateRegisterRequest(CreateCampaignRequestRequest request)
        {
            string currentDateString = TimeHelper.GetTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
            DateTime currentDate = DateTime.ParseExact(currentDateString, "yyyy-MM-dd", null);

            if (request.StartDate < currentDate)
            {
                throw new Exception("Start date must be greater than the current time!");
            }

            if (request.ExpectedEndDate < currentDate)
            {
                throw new Exception("End date must be greater than the current time!");
            }

            if (request.StartDate > request.ExpectedEndDate)
            {
                throw new Exception("End date must be greater than start date!");
            }

            if (!long.TryParse(request.TargetAmount, out long targetAmount))
            {
                throw new Exception("Target amount must be a valid number.");
            }

            if (targetAmount < 0)
            {
                throw new Exception("Target amount must be greater than or equal to 0.");
            }
        }
    }
}
