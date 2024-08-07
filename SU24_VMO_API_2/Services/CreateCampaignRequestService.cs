using BusinessObject.Enums;
using BusinessObject.Models;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Request;

namespace SU24_VMO_API.Services
{
    public class CreateCampaignRequestService
    {
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignTypeRepository _campaignTypeRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IBankingAccountRepository _bankingAccountRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IDonatePhaseRepository _donatePhaseRepository;
        private readonly IProcessingPhaseRepository _processingPhaseRepository;
        private readonly IStatementPhaseRepository _statementPhaseRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly CampaignService _campaignService;
        private readonly FirebaseService _firebaseService;

        public CreateCampaignRequestService(ICreateCampaignRequestRepository createCampaignRequestRepository, FirebaseService firebaseService,
            IMemberRepository memberRepository, IAccountRepository accountRepository, CampaignService campaignService, INotificationRepository notificationRepository,
            IDonatePhaseRepository donatePhaseRepository, IProcessingPhaseRepository processingPhaseRepository, IStatementPhaseRepository statementPhaseRepository,
            IOrganizationManagerRepository organizationManagerRepository, ICampaignRepository campaignRepository, ICampaignTypeRepository campaignTypeRepository,
            IOrganizationRepository organizationRepository, IBankingAccountRepository bankingAccountRepository)
        {
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _firebaseService = firebaseService;
            _memberRepository = memberRepository;
            _accountRepository = accountRepository;
            _campaignService = campaignService;
            _notificationRepository = notificationRepository;
            _donatePhaseRepository = donatePhaseRepository;
            _processingPhaseRepository = processingPhaseRepository;
            _statementPhaseRepository = statementPhaseRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _campaignRepository = campaignRepository;
            _campaignTypeRepository = campaignTypeRepository;
            _organizationRepository = organizationRepository;
            _bankingAccountRepository = bankingAccountRepository;
        }


        public IEnumerable<CreateCampaignRequest> GetCreateCampaignRequests()
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
                if (request.Moderator != null)
                {
                    if (request.Moderator.CreateCampaignRequests != null)
                        request.Moderator.CreateCampaignRequests.Clear();
                    if (request.Moderator.CreatePostRequests != null)
                        request.Moderator.CreatePostRequests.Clear();
                    if (request.Moderator.CreateVolunteerRequests != null)
                        request.Moderator.CreateVolunteerRequests.Clear();
                    if (request.Moderator.CreateActivityRequests != null)
                        request.Moderator.CreateActivityRequests.Clear();
                    if (request.Moderator.CreateOrganizationManagerRequests != null)
                        request.Moderator.CreateOrganizationManagerRequests.Clear();
                    if (request.Moderator.CreateOrganizationRequests != null)
                        request.Moderator.CreateOrganizationRequests.Clear();
                }
                var campaign = _campaignRepository.GetById(request.CampaignID);
                if (campaign != null) request.Campaign = campaign;
                if (request.Campaign != null && request.Campaign.Organization != null)
                {
                    request.Campaign.Organization = null;
                    request.Campaign.CampaignType = null;
                    request.Campaign.Transactions = null;
                }

            }
            return requests;
        }

        public CreateCampaignRequest? GetCreateCampaignRequestById(Guid createCampaignRequestId)
        {
            var request = GetCreateCampaignRequests().FirstOrDefault(c => c.CreateCampaignRequestID.Equals(createCampaignRequestId));
            if (request != null)
            {
                var bankingAccount = _bankingAccountRepository.GetBankingAccountByCampaignId(request.CampaignID);
                if (bankingAccount != null)
                {
                    bankingAccount.Transactions = null;
                    bankingAccount.Account = null;
                    bankingAccount.Campaigns = null;
                }

                if (request.Campaign != null)
                {
                    request.Campaign.BankingAccount = bankingAccount;
                }
            }
            return request;
        }

        public IEnumerable<CreateCampaignRequest>? GetCreateCampaignRequestsByCampaignName(string? campaignName)
        {
            if (!String.IsNullOrEmpty(campaignName))
                return GetCreateCampaignRequests().Where(c => c.Campaign.Name.ToLower().Contains(campaignName.ToLower()));
            else return GetCreateCampaignRequests();
        }

        public async Task<CreateCampaignRequest?> CreateCampaignRequestAsync(Guid accountId, CreateCampaignRequestRequest request, List<Stage>? stages)
        {
            TryValidateRegisterRequest(request);

            var account = _accountRepository.GetById(accountId);
            var om = new OrganizationManager();
            var volunteer = new Member();
            if (account == null)
            {
                return null;
            }
            else
            {
                if (account.Role == Role.OrganizationManager)
                {
                    if (request.CampaignTier == CampaignTier.FullDisbursementCampaign)
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
                            IsTransparent = true,
                            CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                            CanBeDonated = true,
                            IsActive = false,
                            IsModify = false,
                            IsComplete = false,
                            IsDisable = false,
                            CampaignTier = request.CampaignTier,
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

                        campaign.BankingAccountID = bankingAccount.BankingAccountID;

                        var member = _memberRepository.GetByAccountId(accountId);


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
                    else
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
                            IsTransparent = true,
                            CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                            CanBeDonated = true,
                            IsActive = false,
                            IsModify = false,
                            IsComplete = false,
                            IsDisable = false,
                            CampaignTier = request.CampaignTier,
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

                        campaign.BankingAccountID = bankingAccount.BankingAccountID;

                        var member = _memberRepository.GetByAccountId(accountId);


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
                            foreach (var stage in stages)
                            {
                                int i = 0;
                                _processingPhaseRepository.Save(new ProcessingPhase
                                {
                                    ProcessingPhaseId = Guid.NewGuid(),
                                    CampaignId = campaign.CampaignID,
                                    Name = stage.Title,
                                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                    Priority = i,
                                    CurrentMoney = stage.Amount,
                                    Percent = Math.Round((double.Parse(stage.Amount) / double.Parse(campaign.TargetAmount)) * 100, 3),
                                    IsProcessing = false,
                                    IsEnd = false,
                                    IsActive = false,
                                    IsLocked = false,
                                });
                                i++;
                            }
                            _notificationRepository.Save(notification);
                        }
                        return createCampaignRequestCreated;
                    }
                    
                }
                else if (account.Role == Role.Volunteer)
                {
                    if (request.CampaignTier == CampaignTier.FullDisbursementCampaign)
                    {
                        volunteer = _memberRepository.GetByAccountId(accountId);
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
                            IsTransparent = true,
                            CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                            CanBeDonated = true,
                            IsActive = false,
                            IsModify = false,
                            IsComplete = false,
                            IsDisable = false,
                            CampaignTier = request.CampaignTier
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

                        campaign.BankingAccountID = bankingAccount.BankingAccountID;

                        var createCampaignRequest = new CreateCampaignRequest
                        {
                            CreateCampaignRequestID = Guid.NewGuid(),
                            CampaignID = campaign.CampaignID,
                            Campaign = campaign,
                            CreateByMember = volunteer!.MemberID,
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
                        volunteer = _memberRepository.GetByAccountId(accountId);
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
                            IsTransparent = true,
                            CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                            CanBeDonated = true,
                            IsActive = false,
                            IsModify = false,
                            IsComplete = false,
                            IsDisable = false,
                            CampaignTier = request.CampaignTier
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

                        campaign.BankingAccountID = bankingAccount.BankingAccountID;

                        var createCampaignRequest = new CreateCampaignRequest
                        {
                            CreateCampaignRequestID = Guid.NewGuid(),
                            CampaignID = campaign.CampaignID,
                            Campaign = campaign,
                            CreateByMember = volunteer!.MemberID,
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
                            foreach (var stage in stages)
                            {
                                int i = 0;
                                _processingPhaseRepository.Save(new ProcessingPhase
                                {
                                    ProcessingPhaseId = Guid.NewGuid(),
                                    CampaignId = campaign.CampaignID,
                                    Name = stage.Title,
                                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                    Priority = i,
                                    CurrentMoney = stage.Amount,
                                    Percent = Math.Round((double.Parse(stage.Amount) / double.Parse(campaign.TargetAmount)) * 100, 3),
                                    IsProcessing = false,
                                    IsEnd = false,
                                    IsActive = false,
                                    IsLocked = false,
                                });
                                i++;
                            }
                            _notificationRepository.Save(notification);
                        }
                        return createCampaignRequestCreated;
                    }
                }
                else
                {
                    return null;
                }
            }
        }


        public async Task<bool> UpdateCreateCampaignRequest(Guid createCampaignRequestId, UpdateCreateCampaignRequestRequest updateRequest)
        {
            TryValidateUpdateCreateCampaignRequest(updateRequest);
            var requestExisted = _createCampaignRequestRepository.GetById(createCampaignRequestId);
            if (requestExisted == null) throw new NotFoundException("Đơn tạo này không tìm thấy!");
            if (requestExisted.IsApproved) throw new BadRequestException("Đơn tạo chiến dịch này hiện đã được duyệt, vì vậy mọi thông tin về đơn này hiện không thể chỉnh sửa!");
            var campaignExisted = _campaignRepository.GetById(requestExisted.CampaignID);
            if (campaignExisted == null) throw new NotFoundException("Chiến dịch này không tìm thấy!");

            if (!String.IsNullOrEmpty(updateRequest.Description))
            {
                campaignExisted.Description = updateRequest.Description;
            }
            if (!String.IsNullOrEmpty(updateRequest.Address))
            {
                campaignExisted.Address = updateRequest.Address;
            }
            if (!String.IsNullOrEmpty(updateRequest.Name))
            {
                campaignExisted.Name = updateRequest.Name;
            }
            if (!String.IsNullOrEmpty(updateRequest.TargetAmount))
            {
                campaignExisted.TargetAmount = updateRequest.TargetAmount;
            }
            if (updateRequest.CampaignTypeId != null)
            {
                var campaignType = _campaignTypeRepository.GetById((Guid)updateRequest.CampaignTypeId);
                if (campaignType == null) throw new NotFoundException("Loại hình chiến dịch này không tìm thấy!");
                campaignExisted.CampaignTypeID = (Guid)updateRequest.CampaignTypeId;
            }
            if (updateRequest.StartDate != null)
            {
                campaignExisted.StartDate = (DateTime)updateRequest.StartDate;
            }
            if (updateRequest.ExpectedEndDate != null)
            {
                campaignExisted.ExpectedEndDate = (DateTime)updateRequest.ExpectedEndDate;
            }
            if (updateRequest.OrganizationId != null)
            {
                var organization = _organizationRepository.GetById((Guid)updateRequest.OrganizationId);
                if (organization == null) throw new NotFoundException("Tổ chức này không tồn tại!");
                campaignExisted.OrganizationID = (Guid)updateRequest.OrganizationId;
            }
            if (updateRequest.ApplicationConfirmForm != null)
            {
                campaignExisted.ApplicationConfirmForm = await _firebaseService.UploadImage(updateRequest.ApplicationConfirmForm);
            }
            if (updateRequest.ImageCampaign != null)
            {
                campaignExisted.Image = await _firebaseService.UploadImage(updateRequest.ImageCampaign);
            }

            campaignExisted.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
            _campaignRepository.Update(campaignExisted);

            var bankingAccount =
                _bankingAccountRepository.GetBankingAccountByCampaignId(campaignExisted.CampaignID)!;
            if (!String.IsNullOrEmpty(updateRequest.BankingName))
            {
                bankingAccount.BankingName = updateRequest.BankingName;
            }

            if (!String.IsNullOrEmpty(updateRequest.AccountName))
            {
                bankingAccount.AccountName = updateRequest.AccountName;
            }
            if (!String.IsNullOrEmpty(updateRequest.BankingAccountNumber))
            {
                bankingAccount.AccountNumber = updateRequest.BankingAccountNumber;
            }
            if (updateRequest.QRCode != null)
            {
                bankingAccount.QRCode = await _firebaseService.UploadImage(updateRequest.QRCode);
            }

            bankingAccount.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
            _bankingAccountRepository.Update(bankingAccount);
            return true;
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
            var member = new Member();
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

            if (request.CreateByMember != null)
            {
                if (updateCampaignRequest.IsApproved == true)
                {
                    campaign = _campaignService.GetCampaignByCampaignId(request!.CampaignID)!;
                    if (campaign.CampaignTier == CampaignTier.FullDisbursementCampaign)
                    {
                        campaign.IsActive = true;
                        campaign.IsTransparent = true;
                        //campaign.IsDisable = false;

                        request.IsApproved = true;
                        request.IsPending = false;
                        request.IsLocked = false;
                        request.ApprovedBy = updateCampaignRequest.ModeratorId;
                        request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                        request.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                        request.IsRejected = false;
                        result = true;

                        member = _memberRepository.GetById((Guid)request!.CreateByMember!);

                        notification.AccountID = member!.AccountID;
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
                        campaign.IsActive = true;
                        campaign.IsTransparent = true;
                        //campaign.IsDisable = false;

                        request.IsApproved = true;
                        request.IsPending = false;
                        request.IsLocked = false;
                        request.ApprovedBy = updateCampaignRequest.ModeratorId;
                        request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                        request.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                        request.IsRejected = false;
                        result = true;

                        member = _memberRepository.GetById((Guid)request!.CreateByMember!);

                        notification.AccountID = member!.AccountID;
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

                        var processingPhaseExisteds = _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID);
                        if (processingPhaseExisteds != null && processingPhaseExisteds.Any())
                        {
                            //processingPhase = new ProcessingPhase
                            //{
                            //    ProcessingPhaseId = Guid.NewGuid(),
                            //    CampaignId = campaign.CampaignID,
                            //    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                            //    StartDate = TimeHelper.GetTime(DateTime.UtcNow),
                            //    IsProcessing = true,
                            //    IsLocked = false,
                            //    IsEnd = false,
                            //    Name = "Giai đoạn xử lý, giải ngân",
                            //};
                            //_processingPhaseRepository.Save(processingPhase);
                            foreach (var processingPhaseExisted in processingPhaseExisteds)
                            {
                                processingPhaseExisted.StartDate = TimeHelper.GetTime(DateTime.UtcNow);
                                processingPhaseExisted.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                                _processingPhaseRepository.Update(processingPhaseExisted);
                            }
                        }

                        var statementPhaseExisted = _statementPhaseRepository.GetStatementPhaseByCampaignId(campaign.CampaignID);
                        if (statementPhaseExisted == null)
                        {
                            statementPhase = new StatementPhase
                            {
                                StatementPhaseId = Guid.NewGuid(),
                                CampaignId = campaign.CampaignID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                StartDate = TimeHelper.GetTime(DateTime.UtcNow),
                                IsProcessing = true,
                                IsLocked = false,
                                IsEnd = false,
                                Name = "Giai đoạn sao kê",
                            };
                            _statementPhaseRepository.Save(statementPhase);
                        }
                    }


                }
                else
                {
                    campaign = _campaignService.GetCampaignByCampaignId(request!.CampaignID)!;
                    campaign.IsActive = false;
                    campaign.IsTransparent = false;
                    //campaign.IsDisable = true;

                    request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    request.IsApproved = false;
                    request.ApprovedBy = updateCampaignRequest.ModeratorId;
                    request.IsPending = false;
                    request.IsLocked = false;
                    request.IsRejected = true;
                    result = true;

                    member = _memberRepository.GetById((Guid)request!.CreateByMember!);

                    notification.AccountID = member!.AccountID;
                    notification.Content = "Yêu cầu tạo chiến dịch của bạn chưa được chấp thuận! Vui lòng cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
                }
            }
            else
            {
                if (updateCampaignRequest.IsApproved == true)
                {
                    campaign = _campaignService.GetCampaignByCampaignId(request!.CampaignID)!;
                    if (campaign.CampaignTier == CampaignTier.FullDisbursementCampaign)
                    {
                        campaign.IsActive = true;
                        campaign.IsTransparent = true;
                        //campaign.IsDisable = false;



                        request.ApprovedBy = updateCampaignRequest.ModeratorId;
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
                        campaign.IsActive = true;
                        campaign.IsTransparent = true;
                        //campaign.IsDisable = false;



                        request.ApprovedBy = updateCampaignRequest.ModeratorId;
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

                        var processingPhaseExisteds = _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID);
                        if (processingPhaseExisteds != null && processingPhaseExisteds.Any())
                        {
                            //processingPhase = new ProcessingPhase
                            //{
                            //    ProcessingPhaseId = Guid.NewGuid(),
                            //    CampaignId = campaign.CampaignID,
                            //    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                            //    StartDate = TimeHelper.GetTime(DateTime.UtcNow),
                            //    IsProcessing = true,
                            //    IsLocked = false,
                            //    IsEnd = false,
                            //    Name = "Giai đoạn xử lý, giải ngân",
                            //};
                            //_processingPhaseRepository.Save(processingPhase);
                            foreach (var processingPhaseExisted in processingPhaseExisteds)
                            {
                                processingPhaseExisted.StartDate = TimeHelper.GetTime(DateTime.UtcNow);
                                processingPhaseExisted.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                                _processingPhaseRepository.Update(processingPhaseExisted);
                            }
                        }

                        var statementPhaseExisted = _statementPhaseRepository.GetStatementPhaseByCampaignId(campaign.CampaignID);
                        if (statementPhaseExisted == null)
                        {
                            statementPhase = new StatementPhase
                            {
                                StatementPhaseId = Guid.NewGuid(),
                                CampaignId = campaign.CampaignID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                StartDate = TimeHelper.GetTime(DateTime.UtcNow),
                                IsProcessing = true,
                                IsLocked = false,
                                IsEnd = false,
                                Name = "Giai đoạn sao kê",
                            };
                            _statementPhaseRepository.Save(statementPhase);
                        }
                    }
                }
                else
                {
                    campaign = _campaignService.GetCampaignByCampaignId(request!.CampaignID)!;
                    campaign.IsActive = false;
                    campaign.IsTransparent = false;
                    //campaign.IsDisable = true;


                    request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    request.ApprovedBy = updateCampaignRequest.ModeratorId;
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



        private void TryValidateUpdateCreateCampaignRequest(UpdateCreateCampaignRequest request)
        {
            if (String.IsNullOrEmpty(request.CreateCampaignRequestID.ToString()))
            {
                throw new Exception("Id must not be null or empty!");
            }
            if (String.IsNullOrEmpty(request.IsApproved.ToString()))
            {
                throw new Exception("Trạng thái không được để trống!");
            }
        }


        private void TryValidateRegisterRequest(CreateCampaignRequestRequest request)
        {
            string currentDateString = TimeHelper.GetTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
            DateTime currentDate = DateTime.ParseExact(currentDateString, "yyyy-MM-dd", null);

            if (request.StartDate < currentDate)
            {
                throw new Exception("Ngày bắt đầu phải lớn hơn ngày hiện tại!");
            }

            if (request.ExpectedEndDate < currentDate)
            {
                throw new Exception("Ngày kết thúc phải lớn hơn ngày hiện tại!");
            }

            if (request.StartDate > request.ExpectedEndDate)
            {
                throw new Exception("Ngày kết thúc phải lớn hơn ngày bắt đầu!");
            }

            if (!long.TryParse(request.TargetAmount, out long targetAmount))
            {
                throw new Exception("Số tiền mục tiêu định dạng không hợp lệ.");
            }

            if (targetAmount < 0)
            {
                throw new Exception("Số tiền mục tiêu phải lớn hơn hoặc bằng 0.");
            }
        }

        private void TryValidateUpdateCreateCampaignRequest(UpdateCreateCampaignRequestRequest request)
        {
            string currentDateString = TimeHelper.GetTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
            DateTime currentDate = DateTime.ParseExact(currentDateString, "yyyy-MM-dd", null);

            if (request.StartDate < currentDate)
            {
                throw new Exception("Ngày bắt đầu phải lớn hơn ngày hiện tại!");
            }

            if (request.ExpectedEndDate < currentDate)
            {
                throw new Exception("Ngày kết thúc phải lớn hơn ngày hiện tại!");
            }

            if (request.StartDate > request.ExpectedEndDate)
            {
                throw new Exception("Ngày kết thúc phải lớn hơn ngày bắt đầu!");
            }

            if (!long.TryParse(request.TargetAmount, out long targetAmount))
            {
                throw new Exception("Số tiền mục tiêu định dạng không hợp lệ.");
            }

            if (targetAmount < 0)
            {
                throw new Exception("Số tiền mục tiêu phải lớn hơn hoặc bằng 0.");
            }
        }
    }
}
