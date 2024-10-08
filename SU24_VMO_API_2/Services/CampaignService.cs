﻿using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.IdentityModel.Tokens;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Response;
using System.Diagnostics.Metrics;
using System.Text;
using Org.BouncyCastle.Asn1.Cms;
using SU24_VMO_API.Supporters.EmailSupporter;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API_2.DTOs.Request;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SU24_VMO_API.Services
{
    public class CampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignTypeRepository _campaignTypeRepository;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly IMemberRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBankingAccountRepository _bankingAccountRepository;
        private readonly IDonatePhaseRepository _donatePhaseRepository;
        private readonly IActivityImageRepository _activityImageRepository;
        private readonly IProcessingPhaseRepository _processingPhaseRepository;
        private readonly IStatementPhaseRepository _statementPhaseRepository;
        private readonly IStatementFileRepository _statementFileRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IProcessingPhaseStatementFileRepository _processingPhaseStatementFileRepository;
        private readonly IActivityStatementFileRepository _activityStatementFileRepository;
        private readonly FirebaseService _firebaseService;
        private readonly ActivityService _activityService;
        private readonly StatementFileService _statementFileService;

        public CampaignService(ICampaignRepository campaignRepository, FirebaseService firebaseService,
            ICampaignTypeRepository campaignTypeRepository,
            ICreateCampaignRequestRepository createCampaignRequestRepository,
            IOrganizationRepository organizationRepository,
            IDonatePhaseRepository donatePhaseRepository, IProcessingPhaseRepository processingPhaseRepository,
            IStatementPhaseRepository statementPhaseRepository,
            IMemberRepository userRepository, IOrganizationManagerRepository organizationManagerRepository,
            ActivityService activityService, IActivityImageRepository activityImageRepository,
            StatementFileService statementFileService, IStatementFileRepository statementFileRepository,
            IAccountRepository accountRepository, IBankingAccountRepository bankingAccountRepository,
            ITransactionRepository transactionRepository,
            IProcessingPhaseStatementFileRepository processingPhaseStatementFileRepository, IActivityStatementFileRepository activityStatementFileRepository)
        {
            _campaignRepository = campaignRepository;
            _firebaseService = firebaseService;
            _campaignTypeRepository = campaignTypeRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _organizationRepository = organizationRepository;
            _donatePhaseRepository = donatePhaseRepository;
            _processingPhaseRepository = processingPhaseRepository;
            _statementPhaseRepository = statementPhaseRepository;
            _userRepository = userRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _activityService = activityService;
            _activityImageRepository = activityImageRepository;
            _statementFileService = statementFileService;
            _statementFileRepository = statementFileRepository;
            _accountRepository = accountRepository;
            _bankingAccountRepository = bankingAccountRepository;
            _transactionRepository = transactionRepository;
            _processingPhaseStatementFileRepository = processingPhaseStatementFileRepository;
            _activityStatementFileRepository = activityStatementFileRepository;
        }

        public async void UpdateCampaignRequest(Guid campaignId, UpdateCampaignRequest request)
        {
            var campaign = _campaignRepository.GetById(campaignId);
            if (campaign == null)
            {
                throw new NotFoundException("Campaign not found!");
            }

            var campaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaignId);
            if (campaignRequest != null)
            {
                if (campaignRequest.IsApproved)
                    throw new BadRequestException(
                        "Chiến dịch này hiện đã được duyệt, vì vậy mọi thông tin của chiến dịch này không thể chỉnh sửa!");
            }

            if (request.OrganizationID != null)
            {
                var organization = _organizationRepository.GetById((Guid)request.OrganizationID);
                if (organization == null) throw new NotFoundException("Tổ chức không tồn tại!");
                campaign.OrganizationID = request.OrganizationID;
            }

            if (!String.IsNullOrEmpty(request.Name))
            {
                campaign.Name = request.Name;
            }

            if (request.StartDate != null)
            {
                campaign.StartDate = request.StartDate;
            }

            if (!String.IsNullOrEmpty(request.Address))
            {
                campaign.Address = request.Address;
            }

            if (!String.IsNullOrEmpty(request.Description))
            {
                campaign.Description = request.Description;
            }

            if (request.Image != null)
            {
                var image = await _firebaseService.UploadImage(request.Image);
                campaign.Image = image;
            }

            if (!String.IsNullOrEmpty(request.ExpectedEndDate.ToString()))
            {
                campaign.ExpectedEndDate = (DateTime)request.ExpectedEndDate!;
            }

            if (!String.IsNullOrEmpty(request.ApplicationConfirmForm))
            {
                campaign.ApplicationConfirmForm = request.ApplicationConfirmForm!;
            }

            if (!String.IsNullOrEmpty(request.Note))
            {
                campaign.Note = request.Note!;
            }

            if (!String.IsNullOrEmpty(request.IsTransparent.ToString()))
            {
                campaign.IsTransparent = (bool)request.IsTransparent!;
            }

            if (!String.IsNullOrEmpty(request.IsModify.ToString()))
            {
                campaign.IsModify = (bool)request.IsModify!;
            }

            if (!String.IsNullOrEmpty(request.IsComplete.ToString()))
            {
                campaign.IsComplete = (bool)request.IsComplete!;
            }

            if (!String.IsNullOrEmpty(request.CanBeDonated.ToString()))
            {
                campaign.CanBeDonated = (bool)request.CanBeDonated!;
            }

            campaign.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
            _campaignRepository.Update(campaign);
        }

        public void UpdateCampaign(Campaign campaign)
        {
            _campaignRepository.Update(campaign);

        }

        public void UpdateStatusCampaign(UpdateCampaignStatusRequest request)
        {
            var campaign = _campaignRepository.GetById(request.CampaignId);
            if (campaign == null)
            {
                throw new NotFoundException("Không tìm thấy chiến dịch này!");
            }

            var campaignRequest =
                _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(request.CampaignId);
            if (campaignRequest != null)
            {
                if (campaignRequest.IsApproved)
                    throw new BadRequestException(
                        "Chiến dịch này hiện đã được duyệt, vì vậy mọi thông tin của chiến dịch này không thể chỉnh sửa!");
            }

            if (request.IsDisable)
            {
                campaign.IsDisable = true;
                campaign.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
            }

            _campaignRepository.Update(campaign);

        }

        public void UpdateReportCampaign(UpdateReportCampaignRequest request)
        {
            var campaign = _campaignRepository.GetById(request.CampaignId);
            if (campaign == null)
            {
                throw new NotFoundException("Chiến dịch không tồn tại!");
            }

            campaign.IsTransparent = request.IsTransparent;
            campaign.CheckTransparentDate = TimeHelper.GetTime(DateTime.UtcNow);

            var campaignRequest =
                _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);

            if (campaignRequest != null && campaignRequest.CreateByOM != null)
            {
                var om = _organizationManagerRepository.GetById((Guid)campaignRequest.CreateByOM);
                if (om != null)
                {
                    var account = _accountRepository.GetById(om.AccountID);
                    if (account != null && request.IsTransparent)
                    {
                        account.IsActived = false;
                        account.IsBlocked = true;
                        _accountRepository.Update(account);
                    }
                    else if(account != null && !request.IsTransparent)
                    {
                        account.IsActived = true;
                        account.IsBlocked = false;
                        _accountRepository.Update(account);
                    }
                }
            }

            if (campaignRequest != null && campaignRequest.CreateByMember != null)
            {
                var member = _userRepository.GetById((Guid)campaignRequest.CreateByMember);
                if (member != null)
                {
                    var account = _accountRepository.GetById(member.AccountID);
                    if (account != null && request.IsTransparent)
                    {
                        account.IsActived = false;
                        account.IsBlocked = true;
                        _accountRepository.Update(account);
                    }
                    else if (account != null && !request.IsTransparent)
                    {
                        account.IsActived = true;
                        account.IsBlocked = false;
                        _accountRepository.Update(account);
                    }
                }
            }

            _campaignRepository.Update(campaign);
        }


        public CampaignResponseWithAdminTransaction? GetCampaignResponseByCampaignId(Guid campaignId)
        {
            // Fetch campaign and initialize response early
            var campaign = _campaignRepository.GetById(campaignId);
            if (campaign == null) return null;

            var campaignResponse = MapCampaignToResponse(campaign);

            if (campaign.Organization != null)
            {
                CleanUpOrganization(campaign.Organization, campaignResponse);
            }

            if (campaign.CampaignType != null)
            {
                campaign.CampaignType.Campaigns = null;
            }

            if (campaign.DonatePhase != null)
            {
                campaign.DonatePhase.Campaign = null;
            }

            if (campaign.StatementPhase != null)
            {
                campaign.StatementPhase.Campaign = null;
                campaign.StatementPhase.StatementFiles = GetStatementFiles(campaign.StatementPhase.StatementPhaseId);
            }

            var createCampaignRequest =
                _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaignId);
            if (createCampaignRequest?.CreateByMember != null)
            {
                campaignResponse.Member = _userRepository.GetById((Guid)createCampaignRequest.CreateByMember);
            }

            if (campaign.ProcessingPhases != null && campaign.ProcessingPhases.Any())
            {
                campaignResponse.ProcessingPhases = GetProcessingPhases(MapProcessingPhaseToResponse(campaign.ProcessingPhases));
            }


            var transactions = campaignResponse.Transactions;
            if (transactions != null)
            {
                campaignResponse.Transactions = GetFilteredTransactions(transactions, campaignId);
            }

            if (transactions != null)
            {
                campaignResponse.AdminTransactions = GetFilteredAdminTransactions(transactions, campaignId);
            }

            return campaignResponse;
        }

        private CampaignResponseWithAdminTransaction MapCampaignToResponse(Campaign campaign)
        {
            return new CampaignResponseWithAdminTransaction()
            {
                CampaignID = campaign.CampaignID,
                OrganizationID = campaign.OrganizationID,
                CampaignTypeID = campaign.CampaignTypeID,
                ActualEndDate = campaign.ActualEndDate,
                Address = campaign.Address,
                ApplicationConfirmForm = campaign.ApplicationConfirmForm,
                CampaignType = campaign.CampaignType,
                CanBeDonated = campaign.CanBeDonated,
                CheckTransparentDate = campaign.CheckTransparentDate,
                CreateAt = campaign.CreateAt,
                Description = campaign.Description,
                DonatePhase = campaign.DonatePhase,
                ExpectedEndDate = campaign.ExpectedEndDate,
                Image = campaign.Image,
                IsActive = campaign.IsActive,
                IsComplete = campaign.IsComplete,
                IsModify = campaign.IsModify,
                IsTransparent = campaign.IsTransparent,
                Name = campaign.Name,
                Note = campaign.Note,
                Organization = campaign.Organization,
                StartDate = campaign.StartDate,
                StatementPhase = campaign.StatementPhase,
                TargetAmount = campaign.TargetAmount,
                Transactions = campaign.Transactions,
                UpdatedAt = campaign.UpdatedAt,
                CampaignTier = campaign.CampaignTier
            };
        }

        private List<ProcessingPhaseResponseWithAdminTransaction>? MapProcessingPhaseToResponse(List<ProcessingPhase> phases)
        {
            return phases?.Select(phase => new ProcessingPhaseResponseWithAdminTransaction()
            {
                ProcessingPhaseId = phase.ProcessingPhaseId,
                CampaignId = phase.CampaignId,
                Name = phase.Name,
                StartDate = phase.StartDate,
                EndDate = phase.EndDate,
                CreateDate = phase.CreateDate,
                Priority = phase.Priority,
                CurrentMoney = phase.CurrentMoney,
                Percent = phase.Percent,
                CurrentPercent = phase.CurrentPercent,
                IsProcessing = phase.IsProcessing,
                IsEnd = phase.IsEnd,
                UpdateDate = phase.UpdateDate,
                IsLocked = phase.IsLocked,
                IsActive = phase.IsActive,
                UpdateBy = phase.UpdateBy,
                Campaign = phase.Campaign,
                Activities = phase.Activities,
                ProcessingPhaseStatementFiles = phase.ProcessingPhaseStatementFiles,
            }).ToList();
        }

        private void CleanUpOrganization(Organization organization, CampaignResponseWithAdminTransaction campaignResponse)
        {
            organization.Campaigns = null;
            var manager = _organizationManagerRepository.GetById(organization.OrganizationManagerID);
            if (manager != null)
            {
                manager.CreateCampaignRequests = null;
                manager.Organizations = null;
                manager.CreateOrganizationRequests = null;
                manager.CreatePostRequests = null;
                manager.BankingAccounts = null;
            }

            campaignResponse.OrganizationManager = manager;
        }

        private List<StatementFile> GetStatementFiles(Guid statementPhaseId)
        {
            return _statementFileService.GetAll()
                .Where(s => s.StatementPhaseId.Equals(statementPhaseId))
                .Select(s =>
                {
                    s.StatementPhase = null;
                    return s;
                }).ToList();
        }

        private List<ProcessingPhaseResponseWithAdminTransaction> GetProcessingPhases(ICollection<ProcessingPhaseResponseWithAdminTransaction> processingPhases)
        {
            return processingPhases.Select(phase =>
            {
                phase.Campaign = null;
                phase.Activities = GetActivities(phase.ProcessingPhaseId);
                phase.ProcessingPhaseStatementFiles = _processingPhaseStatementFileRepository
                    .GetProcessingPhaseStatementFilesByProcessingPhaseId(phase.ProcessingPhaseId)
                    .ToList();
                phase.AdminTransactions =
                    _transactionRepository.GetTransactionsByProcessingPhaseIdWithTypeIsTransfer(phase.ProcessingPhaseId).ToList();
                return phase;
            }).OrderBy(p => p.Priority).ToList();
        }

        private List<Activity> GetActivities(Guid processingPhaseId)
        {
            var activities = _activityService.GetAllActivityWithProcessingPhaseId(processingPhaseId).ToList();
            if (activities.Any())
            {
                activities.ForEach(a =>
                {
                    a.ProcessingPhase = null;
                    a.ActivityImages = _activityImageRepository.GetAllActivityImagesByActivityId(a.ActivityId).ToList();
                    a.ActivityStatementFiles = _activityStatementFileRepository.GetByActivityId(a.ActivityId).ToList();
                });
            }

            return activities;
        }

        private List<Transaction> GetFilteredTransactions(ICollection<Transaction> transactions, Guid campaignId)
        {
            var adminTransactions = transactions
                .Where(t => t.CampaignID.Equals(campaignId) && t.TransactionStatus == TransactionStatus.Success &&
                            t.TransactionType == TransactionType.Transfer)
                .ToList();

            transactions = transactions
                .Where(t => t.CampaignID.Equals(campaignId) && t.TransactionStatus == TransactionStatus.Success &&
                            t.TransactionType == TransactionType.Receive)
                .ToList();

            transactions.ToList().ForEach(t => t.Campaign = null);

            return transactions.ToList();
        }

        private List<Transaction> GetFilteredAdminTransactions(ICollection<Transaction> transactions, Guid campaignId)
        {
            var adminTransactions = transactions
                .Where(t => t.CampaignID.Equals(campaignId) && t.TransactionStatus == TransactionStatus.Success &&
                            t.TransactionType == TransactionType.Transfer)
                .ToList();

            transactions = transactions
                .Where(t => t.CampaignID.Equals(campaignId) && t.TransactionStatus == TransactionStatus.Success &&
                            t.TransactionType == TransactionType.Receive)
                .ToList();

            adminTransactions.ToList().ForEach(t => t.Campaign = null);

            return adminTransactions.ToList();
        }


        public Campaign? GetCampaignByCampaignId(Guid campaignId)
        {
            var campaign = _campaignRepository.GetById(campaignId);
            if (campaign != null && campaign.Organization != null && campaign.Organization.Campaigns != null)
            {
                campaign.Organization.Campaigns = null;
            }

            if (campaign != null && campaign.CampaignType != null && campaign.CampaignType.Campaigns != null)
            {
                campaign.CampaignType.Campaigns = null;
            }

            return campaign;
        }



        public IEnumerable<Campaign> GetAllCampaigns()
        {
            var campaigns = _campaignRepository.GetAll();
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }

        public IEnumerable<Campaign> GetAllCampaignsForPaging(int pageNumber, int pageSize, string? status,
            Guid? campaignTypeId, string? createBy, string? campaignName)
        {
            var campaigns =
                _campaignRepository.GetAll(pageNumber, pageSize, status, campaignTypeId, createBy, campaignName);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }

        public int CalculateNumberOfCampaignActive()
        {
            var campaignRequests = _createCampaignRequestRepository.GetAll().Where(o => o.IsApproved);
            int count = 0;
            foreach (var request in campaignRequests)
            {
                var campaign = _campaignRepository.GetById(request.CampaignID);
                if (campaign != null)
                {
                    if (campaign.IsActive == true)
                        count++;
                }
            }

            return count;
        }

        public CampaignsTierIWithBankingAccountWithActiveStatus? GetAllCampaignsTierIWithBankingAccountWithActiveStatus(
            string? campaignName, int? pageSize, int? pageNo)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var campaigns = _campaignRepository.GetAllCampaignsTierIWithActiveStatus(campaignName, pageSize, pageNo);
                var campaignsResponse = new List<CampaignWithBankingAccountResponse>();
                foreach (var campaign in campaigns)
                {
                    var bankingAccount = _bankingAccountRepository.GetById(campaign.BankingAccountID);

                    var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID)!;
                    var transaction =
                        _transactionRepository.GetTransactionByCampaignIdWithTypeIsTransfer(campaign.CampaignID);

                    campaignsResponse.Add(new CampaignWithBankingAccountResponse
                    {
                        CampaignID = campaign.CampaignID,
                        BankingAccountId = bankingAccount != null ? bankingAccount.BankingAccountID : null,
                        BankingName = bankingAccount != null ? bankingAccount.BankingName : "không có tên ngân hàng!",
                        AccountName = bankingAccount != null ? bankingAccount.AccountName : "không có tên tài khoản!",
                        QRCode = bankingAccount != null ? bankingAccount.QRCode : "không có mã QR!",
                        BankingAccountNumber = bankingAccount != null
                            ? bankingAccount.AccountNumber
                            : "không có số tài khoản ngân hàng",
                        Amount = donatePhase.CurrentMoney,
                        Percent = donatePhase.Percent,
                        DonatePhaseIsEnd = donatePhase.IsEnd,
                        TransactionImage = transaction != null ? transaction.TransactionImageUrl : null,
                        Name = campaign.Name,
                        IsDisable = campaign.IsDisable,
                        IsActive = campaign.IsActive,
                        IsComplete = campaign.IsComplete
                    });
                }

                return new CampaignsTierIWithBankingAccountWithActiveStatus
                {
                    CampaignWithBankingAccountResponses = campaignsResponse,
                    TotalItem = _campaignRepository.GetAllCampaignsTierIWithActiveStatus(campaignName).Count()
                };
            }
            else
            {
                var campaigns = _campaignRepository.GetAllCampaignsTierIWithActiveStatus(campaignName, pageSize, pageNo);
                var campaignsResponse = new List<CampaignWithBankingAccountResponse>();
                foreach (var campaign in campaigns)
                {
                    var bankingAccount = _bankingAccountRepository.GetById(campaign.BankingAccountID);

                    var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID)!;
                    var transaction =
                        _transactionRepository.GetTransactionByCampaignIdWithTypeIsTransfer(campaign.CampaignID);

                    campaignsResponse.Add(new CampaignWithBankingAccountResponse
                    {
                        CampaignID = campaign.CampaignID,
                        BankingAccountId = bankingAccount != null ? bankingAccount.BankingAccountID : null,
                        BankingName = bankingAccount != null ? bankingAccount.BankingName : "không có tên ngân hàng!",
                        AccountName = bankingAccount != null ? bankingAccount.AccountName : "không có tên tài khoản!",
                        QRCode = bankingAccount != null ? bankingAccount.QRCode : "không có mã QR!",
                        BankingAccountNumber = bankingAccount != null
                            ? bankingAccount.AccountNumber
                            : "Không có số tài khoản ngân hàng",
                        Amount = donatePhase.CurrentMoney,
                        Percent = donatePhase.Percent,
                        DonatePhaseIsEnd = donatePhase.IsEnd,
                        TransactionImage = transaction != null ? transaction.TransactionImageUrl : null,
                        Name = campaign.Name,
                        IsDisable = campaign.IsDisable,
                        IsActive = campaign.IsActive,
                        IsComplete = campaign.IsComplete
                    });
                }

                return new CampaignsTierIWithBankingAccountWithActiveStatus
                {
                    CampaignWithBankingAccountResponses = campaignsResponse,
                    TotalItem = _campaignRepository.GetAllCampaignsTierIWithActiveStatus(campaignName).Count()
                };
            }
        }

        public CampaignsTierIIWithBankingAccountWithActiveStatus?
            GetAllCampaignsTierIIWithBankingAccountWithActiveStatus(string? campaignName, int? pageSize, int? pageNo, string? orderBy)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var campaigns =
                    _campaignRepository.GetAllCampaignsTierIIWithActiveStatus(campaignName);
                var campaignsResponse = new List<CampaignTierIIWithBankingAccountResponse>();
                int total = 0;
                foreach (var campaign in campaigns)
                {
                    var campaignRequest =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (campaignRequest != null && campaignRequest.IsApproved)
                    {
                        var bankingAccount = _bankingAccountRepository.GetById(campaign.BankingAccountID);
                        var processingPhases =
                            _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID);
                        var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID)!;

                        if (processingPhases.Any())
                        {
                            foreach (var processingPhaseIsProcessing in processingPhases)
                            {
                                // Calculate percent and round it to 3 decimal places
                                double targetAmount = double.Parse(campaign.TargetAmount);
                                double currentPercent = campaign.Transactions.Where(t =>
                                    t.TransactionStatus == TransactionStatus.Success &&
                                    t.TransactionType == TransactionType.Receive).Sum(t => t.Amount);
                                processingPhaseIsProcessing.CurrentPercent =
                                    Math.Round((currentPercent / targetAmount) * 100, 3);
                                total++;
                                var transactions =
                                    _transactionRepository.GetTransactionByCampaignTierIIIdWithTypeIsTransfer(
                                        campaign.CampaignID);

                                var listProcessingPhases =
                                                _processingPhaseRepository.GetProcessingPhaseByCampaignId(
                                                    campaign.CampaignID).OrderBy(p => p.Priority);

                                var listProcessingPhaseBeforeCurrentPriority =
                                    listProcessingPhases.Where(p =>
                                        p.Priority <= processingPhaseIsProcessing.Priority);

                                var percentBeforePriority =
                                    listProcessingPhaseBeforeCurrentPriority.Sum(p => p.Percent);
                                if (processingPhaseIsProcessing.CurrentPercent >= Math.Floor((double)percentBeforePriority))
                                {
                                    if (campaignsResponse.FirstOrDefault(p => p.ProcessingPhaseId == processingPhaseIsProcessing.ProcessingPhaseId) == null)
                                        campaignsResponse.Add(new CampaignTierIIWithBankingAccountResponse
                                        {
                                            CampaignID = campaign.CampaignID,
                                            BankingAccountId = bankingAccount != null
                                            ? bankingAccount.BankingAccountID
                                            : null,
                                            BankingName = bankingAccount != null
                                            ? bankingAccount.BankingName
                                            : "không có tên ngân hàng!",
                                            AccountName = bankingAccount != null
                                            ? bankingAccount.AccountName
                                            : "không có tên tài khoản!",
                                            QRCode = bankingAccount != null
                                            ? bankingAccount.QRCode
                                            : "không có mã QR!",
                                            BankingAccountNumber = bankingAccount != null
                                            ? bankingAccount.AccountNumber
                                            : "không có số tài khoản ngân hàng",
                                            Amount = processingPhaseIsProcessing.CurrentMoney,
                                            Percent = donatePhase.Percent,
                                            DonatePhaseIsEnd = donatePhase.IsEnd,
                                            Name = campaign.Name,
                                            IsDisable = campaign.IsDisable,
                                            IsActive = campaign.IsActive,
                                            IsComplete = campaign.IsComplete,
                                            IsProcessing = processingPhaseIsProcessing.IsProcessing,
                                            CurrentMoney = processingPhaseIsProcessing.CurrentMoney,
                                            IsEligible = true,
                                            ProcessingPhaseName = processingPhaseIsProcessing.Name,
                                            ProcessingPhasePercent =
                                            (double)processingPhaseIsProcessing.Percent,
                                            ProcessingPhaseId = processingPhaseIsProcessing.ProcessingPhaseId
                                        });
                                }
                                else
                                {
                                    if (campaignsResponse.FirstOrDefault(p => p.ProcessingPhaseId == processingPhaseIsProcessing.ProcessingPhaseId) == null)
                                        campaignsResponse.Add(new CampaignTierIIWithBankingAccountResponse
                                        {
                                            CampaignID = campaign.CampaignID,
                                            BankingAccountId = bankingAccount != null
                                            ? bankingAccount.BankingAccountID
                                            : null,
                                            BankingName = bankingAccount != null
                                            ? bankingAccount.BankingName
                                            : "không có tên ngân hàng!",
                                            AccountName = bankingAccount != null
                                            ? bankingAccount.AccountName
                                            : "không có tên tài khoản!",
                                            QRCode = bankingAccount != null
                                            ? bankingAccount.QRCode
                                            : "không có mã QR!",
                                            BankingAccountNumber = bankingAccount != null
                                            ? bankingAccount.AccountNumber
                                            : "không có số tài khoản ngân hàng",
                                            Amount = processingPhaseIsProcessing.CurrentMoney,
                                            Percent = donatePhase.Percent,
                                            DonatePhaseIsEnd = donatePhase.IsEnd,
                                            Name = campaign.Name,
                                            IsDisable = campaign.IsDisable,
                                            IsActive = campaign.IsActive,
                                            IsComplete = campaign.IsComplete,
                                            IsProcessing = processingPhaseIsProcessing.IsProcessing,
                                            CurrentMoney = processingPhaseIsProcessing.CurrentMoney,
                                            IsEligible = false,
                                            ProcessingPhaseName = processingPhaseIsProcessing.Name,
                                            ProcessingPhasePercent =
                                            (double)processingPhaseIsProcessing.Percent,
                                            ProcessingPhaseId = processingPhaseIsProcessing.ProcessingPhaseId
                                        });
                                }

                                if (transactions.Any())
                                {

                                    foreach (var transaction in transactions)
                                    {
                                        foreach (var response in campaignsResponse)
                                        {
                                            if (response.ProcessingPhaseId.Equals(transaction.ProcessingPhaseId))
                                            {
                                                response.TransactionImage = transaction.TransactionImageUrl;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                int totalCount = campaignsResponse.Count();

                // Set pageSize to the total count if it's not provided
                int size = pageSize ?? totalCount;
                int page = pageNo ?? 1;

                // Apply pagination
                var query = campaignsResponse
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToList();

                if (!String.IsNullOrEmpty(orderBy) && orderBy.Equals("asc"))
                {
                    query = query.OrderBy(c => c.Name).ToList();
                }
                else if (!String.IsNullOrEmpty(orderBy) && orderBy.Equals("desc"))
                {
                    query = query.OrderByDescending(c => c.Name).ToList();
                }

                return new CampaignsTierIIWithBankingAccountWithActiveStatus
                {
                    CampaignWithBankingAccountResponses = query,
                    TotalItem = total
                };
            }
            else
            {
                var campaigns =
                    _campaignRepository.GetAllCampaignsTierIIWithActiveStatus(campaignName);
                var campaignsResponse = new List<CampaignTierIIWithBankingAccountResponse>();
                int total = 0;
                foreach (var campaign in campaigns)
                {
                    var campaignRequest =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (campaignRequest != null && campaignRequest.IsApproved)
                    {
                        var bankingAccount = _bankingAccountRepository.GetById(campaign.BankingAccountID);
                        var processingPhases =
                            _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID);
                        var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID)!;

                        if (processingPhases.Any())
                        {
                            foreach (var processingPhaseIsProcessing in processingPhases)
                            {
                                // Calculate percent and round it to 3 decimal places
                                double targetAmount = double.Parse(campaign.TargetAmount);
                                double currentPercent = campaign.Transactions.Where(t =>
                                    t.TransactionStatus == TransactionStatus.Success &&
                                    t.TransactionType == TransactionType.Receive).Sum(t => t.Amount);
                                processingPhaseIsProcessing.CurrentPercent =
                                    Math.Round((currentPercent / targetAmount) * 100, 3);
                                total++;
                                var transactions =
                                    _transactionRepository.GetTransactionByCampaignTierIIIdWithTypeIsTransfer(
                                        campaign.CampaignID);

                                var listProcessingPhases =
                                                _processingPhaseRepository.GetProcessingPhaseByCampaignId(
                                                    campaign.CampaignID).OrderBy(p => p.Priority);

                                var listProcessingPhaseBeforeCurrentPriority =
                                    listProcessingPhases.Where(p =>
                                        p.Priority <= processingPhaseIsProcessing.Priority);

                                var percentBeforePriority =
                                    listProcessingPhaseBeforeCurrentPriority.Sum(p => p.Percent);
                                if (processingPhaseIsProcessing.CurrentPercent >= Math.Floor((double)percentBeforePriority))
                                {
                                    if (campaignsResponse.FirstOrDefault(p => p.ProcessingPhaseId == processingPhaseIsProcessing.ProcessingPhaseId) == null)
                                        campaignsResponse.Add(new CampaignTierIIWithBankingAccountResponse
                                        {
                                            CampaignID = campaign.CampaignID,
                                            BankingAccountId = bankingAccount != null
                                            ? bankingAccount.BankingAccountID
                                            : null,
                                            BankingName = bankingAccount != null
                                            ? bankingAccount.BankingName
                                            : "không có tên ngân hàng!",
                                            AccountName = bankingAccount != null
                                            ? bankingAccount.AccountName
                                            : "không có tên tài khoản!",
                                            QRCode = bankingAccount != null
                                            ? bankingAccount.QRCode
                                            : "không có mã QR!",
                                            BankingAccountNumber = bankingAccount != null
                                            ? bankingAccount.AccountNumber
                                            : "không có số tài khoản ngân hàng",
                                            Amount = processingPhaseIsProcessing.CurrentMoney,
                                            Percent = donatePhase.Percent,
                                            DonatePhaseIsEnd = donatePhase.IsEnd,
                                            Name = campaign.Name,
                                            IsDisable = campaign.IsDisable,
                                            IsActive = campaign.IsActive,
                                            IsComplete = campaign.IsComplete,
                                            IsProcessing = processingPhaseIsProcessing.IsProcessing,
                                            CurrentMoney = processingPhaseIsProcessing.CurrentMoney,
                                            IsEligible = true,
                                            ProcessingPhaseName = processingPhaseIsProcessing.Name,
                                            ProcessingPhasePercent =
                                            (double)processingPhaseIsProcessing.Percent,
                                            ProcessingPhaseId = processingPhaseIsProcessing.ProcessingPhaseId
                                        });
                                }
                                else
                                {
                                    if (campaignsResponse.FirstOrDefault(p => p.ProcessingPhaseId == processingPhaseIsProcessing.ProcessingPhaseId) == null)
                                        campaignsResponse.Add(new CampaignTierIIWithBankingAccountResponse
                                        {
                                            CampaignID = campaign.CampaignID,
                                            BankingAccountId = bankingAccount != null
                                            ? bankingAccount.BankingAccountID
                                            : null,
                                            BankingName = bankingAccount != null
                                            ? bankingAccount.BankingName
                                            : "không có tên ngân hàng!",
                                            AccountName = bankingAccount != null
                                            ? bankingAccount.AccountName
                                            : "không có tên tài khoản!",
                                            QRCode = bankingAccount != null
                                            ? bankingAccount.QRCode
                                            : "không có mã QR!",
                                            BankingAccountNumber = bankingAccount != null
                                            ? bankingAccount.AccountNumber
                                            : "không có số tài khoản ngân hàng",
                                            Amount = processingPhaseIsProcessing.CurrentMoney,
                                            Percent = donatePhase.Percent,
                                            DonatePhaseIsEnd = donatePhase.IsEnd,
                                            Name = campaign.Name,
                                            IsDisable = campaign.IsDisable,
                                            IsActive = campaign.IsActive,
                                            IsComplete = campaign.IsComplete,
                                            IsProcessing = processingPhaseIsProcessing.IsProcessing,
                                            CurrentMoney = processingPhaseIsProcessing.CurrentMoney,
                                            IsEligible = false,
                                            ProcessingPhaseName = processingPhaseIsProcessing.Name,
                                            ProcessingPhasePercent =
                                            (double)processingPhaseIsProcessing.Percent,
                                            ProcessingPhaseId = processingPhaseIsProcessing.ProcessingPhaseId
                                        });
                                }

                                if (transactions.Any())
                                {

                                    foreach (var transaction in transactions)
                                    {
                                        foreach (var response in campaignsResponse)
                                        {
                                            if (response.ProcessingPhaseId.Equals(transaction.ProcessingPhaseId))
                                            {
                                                response.TransactionImage = transaction.TransactionImageUrl;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                int totalCount = campaignsResponse.Count();

                // Set pageSize to the total count if it's not provided
                int size = pageSize ?? totalCount;
                int page = pageNo ?? 1;

                // Apply pagination
                var query = campaignsResponse
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToList();

                if (!String.IsNullOrEmpty(orderBy) && orderBy.Equals("asc"))
                {
                    query = query.OrderBy(c => c.Name).ToList();
                }
                else if (!String.IsNullOrEmpty(orderBy) && orderBy.Equals("desc"))
                {
                    query = query.OrderByDescending(c => c.Name).ToList();
                }

                return new CampaignsTierIIWithBankingAccountWithActiveStatus
                {
                    CampaignWithBankingAccountResponses = query,
                    TotalItem = total
                };
            }
        }

        public IEnumerable<Campaign> GetAllCampaignsWithUnActiveStatus()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.IsActive == false);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }




        public IEnumerable<Campaign> GetAllCampaignsWithActiveStatus()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.IsActive == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }

        public IEnumerable<Campaign> GetAllCampaignsTierIIWithActiveStatus()
        {
            var campaigns = _campaignRepository.GetAll().Where(c =>
                c.IsActive == true && c.CampaignTier == CampaignTier.PartialDisbursementCampaign);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }


        public IEnumerable<Campaign> GetAllCampaignsWithEndStatus()
        {
            var campaigns = _campaignRepository.GetAll().Where(c => c.IsComplete == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }


        public IEnumerable<Campaign> GetAllCampaignsWithDonatePhaseWasEnd()
        {
            var campaigns = _campaignRepository.GetAll()
                .Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }

        public IEnumerable<Campaign> GetAllCampaignsByCampaignName(string campaignName)
        {
            var campaigns = _campaignRepository.GetCampaignsByCampaignName(campaignName);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }



        public IEnumerable<Campaign> GetAllCampaignsWithDonatePhaseIsProcessing()
        {
            var campaigns = _campaignRepository.GetAll()
                .Where(c => c.DonatePhase != null && c.DonatePhase.IsProcessing == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }

        public IEnumerable<Campaign> GetAllCampaignsWithProcessingPhaseIsProcessing()
        {
            var campaigns = _campaignRepository.GetAll().Where(c =>
                c.ProcessingPhases != null && c.ProcessingPhases.Any(pp => pp.IsProcessing) == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }


        public IEnumerable<Campaign> GetAllCampaignsWithStatementPhaseIsProcessing()
        {
            var campaigns = _campaignRepository.GetAll()
                .Where(c => c.StatementPhase != null && c.StatementPhase.IsProcessing == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                {
                    campaign.StatementPhase.Campaign = null;
                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                    foreach (var statementFile in statementFiles)
                    {
                        statementFile.StatementPhase = null;
                    }

                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                }
            }

            return campaigns;
        }


        public IEnumerable<Campaign> GetAllCampaignsByCampaignTypeId(Guid campaignTypeId)
        {
            var campaigns = _campaignRepository.GetCampaignsByCampaignTypeId(campaignTypeId);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                    campaign.StatementPhase.Campaign = null;
            }

            return campaigns;
        }


        //public IEnumerable<Campaign> GetAllCampaignsByCampaignTypeIdWithStatus(Guid? campaignTypeId, string? status, string? campaignName, string? createBy)
        //{
        //    if (!String.IsNullOrEmpty(createBy) && createBy.ToLower().Equals("organization"))
        //    {
        //        var campaigns = GetAllCampaigns().Where(c => c.IsActive == true && c.OrganizationID != null);
        //        if (campaignTypeId != null)
        //        {
        //            campaigns = campaigns.Where(c => c.IsActive == true && c.CampaignTypeID.Equals(campaignTypeId));
        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.ProcessingPhases != null)
        //                    campaign.ProcessingPhases = null;
        //                if (campaign.StatementPhase != null)
        //                    campaign.StatementPhase.Campaign = null;
        //            }
        //            if (!String.IsNullOrEmpty(campaignName))
        //            {
        //                campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //            }
        //            return campaigns;
        //        }
        //        else if (!String.IsNullOrEmpty(status))
        //        {
        //            if (campaignTypeId != null)
        //            {
        //                campaigns = campaigns.Where(c => c.IsActive == true && c.CampaignTypeID.Equals(campaignTypeId));
        //                if (!String.IsNullOrEmpty(campaignName))
        //                {
        //                    campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                }
        //                else
        //                {
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                }
        //                return campaigns;
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(campaignName))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true && c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()));
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    return campaigns;
        //                }
        //                else
        //                {
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    else
        //                    {
        //                        return campaigns.Where(c => c.IsActive == true);
        //                    }
        //                }
        //            }
        //        }
        //        else if (!String.IsNullOrEmpty(campaignName))
        //        {
        //            campaigns = campaigns.Where(c => c.IsActive == true && c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()));
        //            if (campaignTypeId != null)
        //            {
        //                campaigns = campaigns.Where(c => c.IsActive == true && campaignTypeId.Equals(campaignTypeId));
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //                return campaigns;
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //                return campaigns;
        //            }
        //        }
        //        else
        //        {
        //            return campaigns.Where(c => c.IsActive == true);
        //        }
        //    }
        //    else if (!String.IsNullOrEmpty(createBy) && createBy.ToLower().Equals("volunteer"))
        //    {
        //        var campaigns = GetAllCampaigns().Where(c => c.IsActive == true && c.OrganizationID == null);
        //        if (campaignTypeId != null)
        //        {
        //            campaigns = campaigns.Where(c => c.IsActive == true && c.CampaignTypeID.Equals(campaignTypeId));
        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.ProcessingPhases != null)
        //                    campaign.ProcessingPhases = null;
        //                if (campaign.StatementPhase != null)
        //                    campaign.StatementPhase.Campaign = null;
        //            }
        //            if (!String.IsNullOrEmpty(campaignName))
        //            {
        //                campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //            }
        //            return campaigns;
        //        }
        //        else if (!String.IsNullOrEmpty(status))
        //        {
        //            if (campaignTypeId != null)
        //            {
        //                campaigns = campaigns.Where(c => c.IsActive == true && c.CampaignTypeID.Equals(campaignTypeId));
        //                if (!String.IsNullOrEmpty(campaignName))
        //                {
        //                    campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                }
        //                else
        //                {
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                }
        //                return campaigns;
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(campaignName))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true && c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()));
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    return campaigns;
        //                }
        //                else
        //                {
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    else
        //                    {
        //                        return campaigns.Where(c => c.IsActive == true);
        //                    }
        //                }
        //            }
        //        }
        //        else if (!String.IsNullOrEmpty(campaignName))
        //        {
        //            campaigns = campaigns.Where(c => c.IsActive == true && c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()));
        //            if (campaignTypeId != null)
        //            {
        //                campaigns = campaigns.Where(c => c.IsActive == true && campaignTypeId.Equals(campaignTypeId));
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //                return campaigns;
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //                return campaigns;
        //            }
        //        }
        //        else
        //        {
        //            return campaigns.Where(c => c.IsActive == true);
        //        }
        //    }
        //    else
        //    {
        //        if (campaignTypeId != null)
        //        {
        //            var campaigns = _campaignRepository.GetCampaignsByCampaignTypeId((Guid)campaignTypeId).Where(c => c.IsActive == true);
        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.ProcessingPhases != null)
        //                    campaign.ProcessingPhases = null;
        //                if (campaign.StatementPhase != null)
        //                    campaign.StatementPhase.Campaign = null;
        //            }
        //            if (!String.IsNullOrEmpty(campaignName))
        //            {
        //                campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //            }
        //            return campaigns;
        //        }
        //        else if (!String.IsNullOrEmpty(status))
        //        {
        //            if (campaignTypeId != null)
        //            {
        //                var campaigns = _campaignRepository.GetCampaignsByCampaignTypeId((Guid)campaignTypeId).Where(c => c.IsActive == true);
        //                if (!String.IsNullOrEmpty(campaignName))
        //                {
        //                    campaigns = campaigns.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()) && c.IsActive == true);
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                }
        //                else
        //                {
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                }
        //                return campaigns;
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(campaignName))
        //                {
        //                    var campaigns = GetAllCampaignsByCampaignName(campaignName).Where(c => c.IsActive == true);
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsActive == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }

        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        campaigns = campaigns.Where(c => c.IsComplete == true);
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    return campaigns;
        //                }
        //                else
        //                {
        //                    if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                    {
        //                        var campaigns = GetAllCampaignsWithActiveStatus();
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                    {
        //                        var campaigns = GetAllCampaignsWithDonatePhaseWasEnd();
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    else if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                    {
        //                        var campaigns = GetAllCampaignsWithEndStatus();
        //                        foreach (var campaign in campaigns)
        //                        {
        //                            if (campaign.CampaignType != null)
        //                                campaign.CampaignType!.Campaigns = null;
        //                            if (campaign.Organization != null)
        //                                campaign.Organization!.Campaigns = null;
        //                            if (campaign.ProcessingPhases != null)
        //                                campaign.ProcessingPhases = null;
        //                            if (campaign.StatementPhase != null)
        //                                campaign.StatementPhase.Campaign = null;
        //                        }
        //                        return campaigns;
        //                    }
        //                    else
        //                    {
        //                        return GetAllCampaigns().Where(c => c.IsActive == true);
        //                    }
        //                }
        //            }
        //        }
        //        else if (!String.IsNullOrEmpty(campaignName))
        //        {
        //            var campaigns = GetAllCampaignsByCampaignName(campaignName).Where(c => c.IsActive == true);
        //            if (campaignTypeId != null)
        //            {
        //                campaigns = campaigns.Where(c => c.IsActive == true && campaignTypeId.Equals(campaignTypeId));
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //                return campaigns;
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đang thực hiện".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsActive == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đạt mục tiêu".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }

        //                if (!String.IsNullOrEmpty(status) && status.ToLower().Trim().Equals("Đã kết thúc".Trim().ToLower()))
        //                {
        //                    campaigns = campaigns.Where(c => c.IsComplete == true);
        //                    foreach (var campaign in campaigns)
        //                    {
        //                        if (campaign.CampaignType != null)
        //                            campaign.CampaignType!.Campaigns = null;
        //                        if (campaign.Organization != null)
        //                            campaign.Organization!.Campaigns = null;
        //                        if (campaign.ProcessingPhases != null)
        //                            campaign.ProcessingPhases = null;
        //                        if (campaign.StatementPhase != null)
        //                            campaign.StatementPhase.Campaign = null;
        //                    }
        //                    return campaigns;
        //                }
        //                return campaigns;
        //            }
        //        }
        //        else
        //        {
        //            return GetAllCampaigns().Where(c => c.IsActive == true);
        //        }
        //    }
        //}

        public IEnumerable<Campaign> GetAllCampaignsByCampaignTypeIdWithStatus(Guid? campaignTypeId, string? status,
            string? campaignName, string? createBy, int pageNumber, int pageSize)
        {
            var campaigns =
                GetAllCampaignsForPaging(pageNumber, pageSize, status, campaignTypeId, createBy, campaignName);
            foreach (var campaign in campaigns)
            {
                NullifyNavigationProperties(campaign);
            }

            return campaigns;
        }

        private void NullifyNavigationProperties(Campaign campaign)
        {
            if (campaign.CampaignType != null)
                campaign.CampaignType!.Campaigns = null;
            if (campaign.Organization != null)
                campaign.Organization!.Campaigns = null;
            campaign.ProcessingPhases = null;
            if (campaign.StatementPhase != null)
                campaign.StatementPhase.Campaign = null;
        }

        public IEnumerable<CampaignResponseWithAdminTransaction> GetAllCampaignResponsesByCampaignTypeIdWithStatus(Guid? campaignTypeId,
            string? status, string? campaignName, string? createBy, int pageNumber, int pageSize)
        {
            var listCampaigns = GetAllCampaignsByCampaignTypeIdWithStatus(campaignTypeId, status, campaignName,
                createBy, pageNumber, pageSize);
            var listCampaignsResponse = new List<CampaignResponseWithAdminTransaction>();
            foreach (var campaign in listCampaigns)
            {
                var response = GetCampaignResponseByCampaignId(campaign.CampaignID);
                if (response != null)
                {
                    response.AdminTransactions = null;

                    if (response.StatementPhase != null)
                        response.StatementPhase.StatementFiles = null;
                    response.Transactions = null;
                    if (response.ProcessingPhases is { Count: > 0 })
                    {
                        foreach (var processingPhase in response.ProcessingPhases)
                        {
                            processingPhase.Activities = null;
                        }
                    }

                    listCampaignsResponse.Add(response);
                }
            }

            return listCampaignsResponse;
        }




        public IEnumerable<Campaign> GetAllCampaignsWithActiveStatusByCampaignTypeId(Guid campaignTypeId)
        {
            var campaigns = _campaignRepository.GetCampaignsByCampaignTypeId(campaignTypeId)
                .Where(c => c.IsActive == true);
            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.ProcessingPhases != null)
                    campaign.ProcessingPhases = null;
                if (campaign.StatementPhase != null)
                {
                    campaign.StatementPhase.Campaign = null;
                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                    foreach (var statementFile in statementFiles)
                    {
                        statementFile.StatementPhase = null;
                    }

                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                }
            }

            return campaigns;
        }

        public CampaignCreateByOm? GetAllCampaignByCreateByOrganizationManagerId(Guid organizationManagerId,
            string? campaignName, int? pageSize, int? pageNo)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(
                        organizationManagerId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        if (createCampaignRequest.Campaign != null &&
                            createCampaignRequest.Campaign.OrganizationID != null)
                        {
                            var organization =
                                _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                            createCampaignRequest.Campaign!.Organization = organization;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }
                }

                return new CampaignCreateByOm
                {
                    Campaigns = campaigns.Where(c =>
                        c.Name.ToLower().Contains(campaignName.ToLower().Trim()) && c.IsDisable == false &&
                        c.CampaignTier == CampaignTier.FullDisbursementCampaign).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId).Where(c => c.Campaign.CampaignTier == CampaignTier.FullDisbursementCampaign).Count()
                };
            }
            else
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(
                        organizationManagerId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        if (createCampaignRequest.Campaign != null &&
                            createCampaignRequest.Campaign.OrganizationID != null)
                        {
                            var organization =
                                _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                            createCampaignRequest.Campaign!.Organization = organization;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }

                    var request =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (request != null)
                    {
                        request.Campaign = null;
                        request.OrganizationManager = null;
                        request.Member = null;
                        request.Moderator = null;
                        campaign.CreateCampaignRequest = request;
                    }


                }

                return new CampaignCreateByOm
                {
                    Campaigns = campaigns.Where(c =>
                        c.IsDisable == false && c.CampaignTier == CampaignTier.FullDisbursementCampaign).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId).Where(c => c.Campaign.CampaignTier == CampaignTier.FullDisbursementCampaign).Count()
                };
            }

        }

        public CampaignCreateByOm? GetAllCampaignTierIAndTierIIByCreateByOrganizationManagerId(
            Guid organizationManagerId, string? campaignName, int? pageSize, int? pageNo)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(
                        organizationManagerId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        if (createCampaignRequest.Campaign != null &&
                            createCampaignRequest.Campaign.OrganizationID != null)
                        {
                            var organization =
                                _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                            createCampaignRequest.Campaign!.Organization = organization;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }
                }

                return new CampaignCreateByOm
                {
                    Campaigns = campaigns.Where(c =>
                        c.Name.ToLower().Contains(campaignName.ToLower().Trim()) && c.IsDisable == false).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId).Count()
                };
            }
            else
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(
                        organizationManagerId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        if (createCampaignRequest.Campaign != null &&
                            createCampaignRequest.Campaign.OrganizationID != null)
                        {
                            var organization =
                                _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                            createCampaignRequest.Campaign!.Organization = organization;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }

                    var request =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (request != null)
                    {
                        request.Campaign = null;
                        request.OrganizationManager = null;
                        request.Member = null;
                        request.Moderator = null;
                        campaign.CreateCampaignRequest = request;
                    }


                }

                return new CampaignCreateByOm
                {
                    Campaigns = campaigns.Where(c => c.IsDisable == false).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId).Count()
                };
            }

        }

        public CampaignCreateByOm? GetAllCampaignTierIICreateByOrganizationManagerId(Guid organizationManagerId,
            string? campaignName, int? pageSize, int? pageNo)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(
                        organizationManagerId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        if (createCampaignRequest.Campaign != null &&
                            createCampaignRequest.Campaign.OrganizationID != null)
                        {
                            var organization =
                                _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                            createCampaignRequest.Campaign!.Organization = organization;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }
                }

                return new CampaignCreateByOm
                {
                    Campaigns = campaigns.Where(c =>
                        c.Name.ToLower().Contains(campaignName.ToLower().Trim()) && c.IsDisable == false &&
                        c.CampaignTier == CampaignTier.PartialDisbursementCampaign).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId).Where(c => c.Campaign.CampaignTier == CampaignTier.PartialDisbursementCampaign).Count()
                };
            }
            else
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(
                        organizationManagerId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        if (createCampaignRequest.Campaign != null &&
                            createCampaignRequest.Campaign.OrganizationID != null)
                        {
                            var organization =
                                _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                            createCampaignRequest.Campaign!.Organization = organization;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }

                    var request =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (request != null)
                    {
                        request.Campaign = null;
                        request.OrganizationManager = null;
                        request.Member = null;
                        request.Moderator = null;
                        campaign.CreateCampaignRequest = request;
                    }


                }

                return new CampaignCreateByOm
                {
                    Campaigns = campaigns.Where(c =>
                        c.IsDisable == false && c.CampaignTier == CampaignTier.PartialDisbursementCampaign).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId).Where(c => c.Campaign.CampaignTier == CampaignTier.PartialDisbursementCampaign).Count()
                };
            }

        }





        public CampaignCreateByVolunteerResponse? GetAllCampaignByCreateByVolunteerId(Guid userId, string? campaignName,
            int? pageSize, int? pageNo)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        campaigns.Add(createCampaignRequest.Campaign!);
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }
                }

                return new CampaignCreateByVolunteerResponse
                {
                    Campaigns = campaigns.Where(c =>
                        c.Name.ToLower().Contains(campaignName.ToLower().Trim()) && c.IsDisable == false &&
                        c.CampaignTier == CampaignTier.FullDisbursementCampaign).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId).Where(c => c.Campaign.CampaignTier == CampaignTier.FullDisbursementCampaign).Count()
                };
            }
            else
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        campaigns.Add(createCampaignRequest.Campaign!);
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }

                    var request =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (request != null)
                    {
                        request.Campaign = null;
                        request.OrganizationManager = null;
                        request.Member = null;
                        request.Moderator = null;
                        campaign.CreateCampaignRequest = request;
                    }
                }

                return new CampaignCreateByVolunteerResponse
                {
                    Campaigns = campaigns.Where(c =>
                        c.IsDisable == false && c.CampaignTier == CampaignTier.FullDisbursementCampaign).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId).Where(c => c.Campaign.CampaignTier == CampaignTier.FullDisbursementCampaign).Count()
                };
            }
        }


        public IEnumerable<Campaign?> GetAllCampaignTierIAndTierIIByCreateByVolunteerId(Guid userId,
            string? campaignName, int? pageSize, int? pageNo)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        campaigns.Add(createCampaignRequest.Campaign!);
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }
                }

                return campaigns.Where(c =>
                    c.Name.ToLower().Contains(campaignName.ToLower().Trim()) && c.IsDisable == false);
            }
            else
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        campaigns.Add(createCampaignRequest.Campaign!);
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }

                    var request =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (request != null)
                    {
                        request.Campaign = null;
                        request.OrganizationManager = null;
                        request.Member = null;
                        request.Moderator = null;
                        campaign.CreateCampaignRequest = request;
                    }
                }

                return campaigns.Where(c => c.IsDisable == false);
            }
        }

        public CampaignCreateByVolunteerResponse? GetAllCampaignTierIIByCreateByVolunteerId(Guid userId, string? campaignName,
            int? pageSize, int? pageNo)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        campaigns.Add(createCampaignRequest.Campaign!);
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }
                }

                return new CampaignCreateByVolunteerResponse
                {
                    Campaigns = campaigns.Where(c =>
                        c.Name.ToLower().Contains(campaignName.ToLower().Trim()) && c.IsDisable == false &&
                        c.CampaignTier == CampaignTier.PartialDisbursementCampaign).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId).Where(c => c.Campaign.CampaignTier == CampaignTier.PartialDisbursementCampaign).Count()
                };
            }
            else
            {
                var createCampaignRequests =
                    _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId, pageSize, pageNo);
                var campaigns = new List<Campaign>();
                if (createCampaignRequests != null)
                {
                    foreach (var createCampaignRequest in createCampaignRequests)
                    {
                        campaigns.Add(createCampaignRequest.Campaign!);
                    }
                }

                foreach (var campaign in campaigns)
                {
                    if (campaign.CampaignType != null)
                        campaign.CampaignType!.Campaigns = null;
                    if (campaign.Organization != null)
                        campaign.Organization.OrganizationManager = null;
                    if (campaign.Organization != null)
                        campaign.Organization!.Campaigns = null;
                    if (campaign.ProcessingPhases != null)
                        campaign.ProcessingPhases = null;
                    if (campaign.StatementPhase != null)
                    {
                        campaign.StatementPhase.Campaign = null;
                        var statementFiles = _statementFileRepository.GetAll().Where(s =>
                            s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                        foreach (var statementFile in statementFiles)
                        {
                            statementFile.StatementPhase = null;
                        }

                        campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                    }

                    var request =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (request != null)
                    {
                        request.Campaign = null;
                        request.OrganizationManager = null;
                        request.Member = null;
                        request.Moderator = null;
                        campaign.CreateCampaignRequest = request;
                    }
                }

                return new CampaignCreateByVolunteerResponse
                {
                    Campaigns = campaigns.Where(c =>
                        c.IsDisable == false && c.CampaignTier == CampaignTier.PartialDisbursementCampaign).ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId).Where(c => c.Campaign.CampaignTier == CampaignTier.PartialDisbursementCampaign).Count()
                };
            }
        }

        public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerIdWithDonatePhaseIsProcessing(
            Guid organizationManagerId, int? pageSize, int? pageNo)
        {
            var createCampaignRequests =
                _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(
                    organizationManagerId, pageSize, pageNo);
            var campaigns = new List<Campaign>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                    {
                        var organization =
                            _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                        createCampaignRequest.Campaign!.Organization = organization;
                        var donatePhase =
                            _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
                        if (donatePhase != null && donatePhase.IsProcessing == true)
                        {
                            createCampaignRequest.Campaign!.Organization = organization;
                            createCampaignRequest.Campaign!.DonatePhase = donatePhase;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.DonatePhase != null)
                    campaign.DonatePhase.Campaign = null;
                if (campaign.ProcessingPhases != null)
                    foreach (var processingPhase in campaign.ProcessingPhases)
                    {
                        processingPhase.Campaign = null;
                    }

                if (campaign.StatementPhase != null)
                {
                    campaign.StatementPhase.Campaign = null;
                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                    foreach (var statementFile in statementFiles)
                    {
                        statementFile.StatementPhase = null;
                    }

                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                }

                if (campaign.CreateCampaignRequest != null)
                {
                    campaign.CreateCampaignRequest.OrganizationManager = null;
                    campaign.CreateCampaignRequest.Member = null;
                    campaign.CreateCampaignRequest.Moderator = null;
                }
            }

            return campaigns;
        }


        //public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerIdWithOptionsPhaseInProcessingPhase(Guid organizationManagerId, string? status, string? campaignName)
        //{
        //    if (!String.IsNullOrEmpty(campaignName))
        //    {
        //        string normalizedCampaignName = campaignName.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        //        if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("donate-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
        //            var campaigns = new List<Campaign>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
        //                    {
        //                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
        //                        createCampaignRequest.Campaign!.Organization = organization;
        //                        var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
        //                        if (donatePhase != null && donatePhase.IsProcessing == true)
        //                        {
        //                            createCampaignRequest.Campaign!.Organization = organization;
        //                            createCampaignRequest.Campaign!.DonatePhase = donatePhase;
        //                            campaigns.Add(createCampaignRequest.Campaign!);
        //                        }
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processingPhase in campaign.ProcessingPhases)
        //                    {
        //                        processingPhase.Campaign = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }

        //                if (campaign.CreateCampaignRequest != null)
        //                {
        //                    campaign.CreateCampaignRequest.OrganizationManager = null;
        //                    campaign.CreateCampaignRequest.Member = null;
        //                    campaign.CreateCampaignRequest.Moderator = null;
        //                }
        //            }
        //            return campaigns.Where(a => a.Name.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedCampaignName));
        //        }
        //        else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("processing-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
        //            var campaigns = new List<Campaign>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
        //                    {
        //                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
        //                        createCampaignRequest.Campaign!.Organization = organization;
        //                        var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
        //                        if (processingPhase != null && processingPhase.Any(pp => pp.IsProcessing) == true)
        //                        {
        //                            createCampaignRequest.Campaign!.Organization = organization;
        //                            createCampaignRequest.Campaign!.ProcessingPhases = processingPhase;
        //                            campaigns.Add(createCampaignRequest.Campaign!);
        //                        }
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processingPhase in campaign.ProcessingPhases)
        //                    {
        //                        processingPhase.Campaign = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //                if (campaign.CreateCampaignRequest != null)
        //                {
        //                    campaign.CreateCampaignRequest.OrganizationManager = null;
        //                    campaign.CreateCampaignRequest.Member = null;
        //                    campaign.CreateCampaignRequest.Moderator = null;
        //                }
        //            }
        //            return campaigns.Where(a => a.Name.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedCampaignName));
        //        }
        //        else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("statement-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
        //            var campaigns = new List<Campaign>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
        //                    {
        //                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
        //                        createCampaignRequest.Campaign!.Organization = organization;
        //                        var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
        //                        if (statementPhase != null && statementPhase.IsProcessing == true)
        //                        {
        //                            createCampaignRequest.Campaign!.Organization = organization;
        //                            createCampaignRequest.Campaign!.StatementPhase = statementPhase;
        //                            campaigns.Add(createCampaignRequest.Campaign!);
        //                        }
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processingPhase in campaign.ProcessingPhases)
        //                    {
        //                        processingPhase.Campaign = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //                if (campaign.CreateCampaignRequest != null)
        //                {
        //                    campaign.CreateCampaignRequest.OrganizationManager = null;
        //                    campaign.CreateCampaignRequest.Member = null;
        //                    campaign.CreateCampaignRequest.Moderator = null;
        //                }
        //            }
        //            return campaigns.Where(a => a.Name.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedCampaignName));
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("Giai đoạn này không hợp lệ!");
        //        }
        //    }
        //    else
        //    {
        //        if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("donate-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
        //            var campaigns = new List<Campaign>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
        //                    {
        //                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
        //                        createCampaignRequest.Campaign!.Organization = organization;
        //                        var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
        //                        if (donatePhase != null && donatePhase.IsProcessing == true)
        //                        {
        //                            createCampaignRequest.Campaign!.Organization = organization;
        //                            createCampaignRequest.Campaign!.DonatePhase = donatePhase;
        //                            campaigns.Add(createCampaignRequest.Campaign!);
        //                        }
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processingPhase in campaign.ProcessingPhases)
        //                    {
        //                        processingPhase.Campaign = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //                if (campaign.CreateCampaignRequest != null)
        //                {
        //                    campaign.CreateCampaignRequest.OrganizationManager = null;
        //                    campaign.CreateCampaignRequest.Member = null;
        //                    campaign.CreateCampaignRequest.Moderator = null;
        //                }
        //            }
        //            return campaigns;
        //        }
        //        else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("processing-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
        //            var campaigns = new List<Campaign>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
        //                    {
        //                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
        //                        createCampaignRequest.Campaign!.Organization = organization;
        //                        var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
        //                        if (processingPhase != null && processingPhase.Any(pp => pp.IsProcessing) == true)
        //                        {
        //                            createCampaignRequest.Campaign!.Organization = organization;
        //                            createCampaignRequest.Campaign!.ProcessingPhases = processingPhase;
        //                            campaigns.Add(createCampaignRequest.Campaign!);
        //                        }
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processingPhase in campaign.ProcessingPhases)
        //                    {
        //                        processingPhase.Campaign = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //                if (campaign.CreateCampaignRequest != null)
        //                {
        //                    campaign.CreateCampaignRequest.OrganizationManager = null;
        //                    campaign.CreateCampaignRequest.Member = null;
        //                    campaign.CreateCampaignRequest.Moderator = null;
        //                }
        //            }
        //            return campaigns;
        //        }
        //        else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("statement-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId);
        //            var campaigns = new List<Campaign>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
        //                    {
        //                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
        //                        createCampaignRequest.Campaign!.Organization = organization;
        //                        var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
        //                        if (statementPhase != null && statementPhase.IsProcessing == true)
        //                        {
        //                            createCampaignRequest.Campaign!.Organization = organization;
        //                            createCampaignRequest.Campaign!.StatementPhase = statementPhase;
        //                            campaigns.Add(createCampaignRequest.Campaign!);
        //                        }
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processingPhase in campaign.ProcessingPhases)
        //                    {
        //                        processingPhase.Campaign = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //                if (campaign.CreateCampaignRequest != null)
        //                {
        //                    campaign.CreateCampaignRequest.OrganizationManager = null;
        //                    campaign.CreateCampaignRequest.Member = null;
        //                    campaign.CreateCampaignRequest.Moderator = null;
        //                }
        //            }
        //            return campaigns;
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("Giai đoạn này không hợp lệ!");
        //        }
        //    }

        //}


        //public IEnumerable<CampaignResponse?> GetAllCampaignByCreateByVolunteerIdWithOptionsPhaseInProcessingPhase(Guid userId, string? status, string? campaignName)
        //{
        //    if (!String.IsNullOrEmpty(campaignName))
        //    {
        //        string normalizedCampaignName = campaignName.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        //        if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("donate-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId);
        //            var campaigns = new List<CampaignResponse>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    var member = _userRepository.GetById(userId);
        //                    var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
        //                    if (donatePhase != null && donatePhase.IsProcessing == true)
        //                    {
        //                        createCampaignRequest.Campaign!.DonatePhase = donatePhase;
        //                        campaigns.Add(new CampaignResponse
        //                        {
        //                            CampaignID = createCampaignRequest.Campaign!.CampaignID,
        //                            OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
        //                            CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
        //                            ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
        //                            Address = createCampaignRequest.Campaign!.Address,
        //                            ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
        //                            CampaignType = createCampaignRequest.Campaign!.CampaignType,
        //                            CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
        //                            CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
        //                            CreateAt = createCampaignRequest.Campaign!.CreateAt,
        //                            Description = createCampaignRequest.Campaign!.Description,
        //                            DonatePhase = donatePhase,
        //                            ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
        //                            Image = createCampaignRequest.Campaign!.Image,
        //                            IsActive = createCampaignRequest.Campaign!.IsActive,
        //                            IsComplete = createCampaignRequest.Campaign!.IsComplete,
        //                            IsModify = createCampaignRequest.Campaign!.IsModify,
        //                            IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
        //                            Name = createCampaignRequest.Campaign!.Name,
        //                            Note = createCampaignRequest.Campaign!.Note,
        //                            Organization = createCampaignRequest.Campaign!.Organization,
        //                            ProcessingPhases = createCampaignRequest.Campaign!.ProcessingPhases,
        //                            StartDate = createCampaignRequest.Campaign!.StartDate,
        //                            StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
        //                            TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
        //                            Transactions = createCampaignRequest.Campaign!.Transactions,
        //                            UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
        //                            Member = member,
        //                            CampaignTier = createCampaignRequest.Campaign!.CampaignTier
        //                        });
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processing in campaign.ProcessingPhases)
        //                    {
        //                        processing.Activities = null;
        //                        processing.Campaign = null;
        //                        processing.ProcessingPhaseStatementFiles = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //            }
        //            return campaigns.Where(a => a.Name.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedCampaignName));
        //        }
        //        else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("processing-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId);
        //            var campaigns = new List<CampaignResponse>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    var member = _userRepository.GetById(userId);
        //                    var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
        //                    if (processingPhase != null && processingPhase.Any(pp => pp.IsProcessing) == true)
        //                    {
        //                        campaigns.Add(new CampaignResponse
        //                        {
        //                            CampaignID = createCampaignRequest.Campaign!.CampaignID,
        //                            OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
        //                            CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
        //                            ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
        //                            Address = createCampaignRequest.Campaign!.Address,
        //                            ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
        //                            CampaignType = createCampaignRequest.Campaign!.CampaignType,
        //                            CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
        //                            CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
        //                            CreateAt = createCampaignRequest.Campaign!.CreateAt,
        //                            Description = createCampaignRequest.Campaign!.Description,
        //                            DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
        //                            ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
        //                            Image = createCampaignRequest.Campaign!.Image,
        //                            IsActive = createCampaignRequest.Campaign!.IsActive,
        //                            IsComplete = createCampaignRequest.Campaign!.IsComplete,
        //                            IsModify = createCampaignRequest.Campaign!.IsModify,
        //                            IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
        //                            Name = createCampaignRequest.Campaign!.Name,
        //                            Note = createCampaignRequest.Campaign!.Note,
        //                            Organization = createCampaignRequest.Campaign!.Organization,
        //                            ProcessingPhases = processingPhase,
        //                            StartDate = createCampaignRequest.Campaign!.StartDate,
        //                            StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
        //                            TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
        //                            Transactions = createCampaignRequest.Campaign!.Transactions,
        //                            UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
        //                            Member = member,
        //                            CampaignTier = createCampaignRequest.Campaign!.CampaignTier
        //                        });
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processing in campaign.ProcessingPhases)
        //                    {
        //                        processing.Activities = null;
        //                        processing.Campaign = null;
        //                        processing.ProcessingPhaseStatementFiles = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //            }
        //            return campaigns.Where(a => a.Name.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedCampaignName));
        //        }
        //        else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("statement-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId);
        //            var campaigns = new List<CampaignResponse>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    var member = _userRepository.GetById(userId);
        //                    var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
        //                    if (statementPhase != null && statementPhase.IsProcessing == true)
        //                    {
        //                        campaigns.Add(new CampaignResponse
        //                        {
        //                            CampaignID = createCampaignRequest.Campaign!.CampaignID,
        //                            OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
        //                            CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
        //                            ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
        //                            Address = createCampaignRequest.Campaign!.Address,
        //                            ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
        //                            CampaignType = createCampaignRequest.Campaign!.CampaignType,
        //                            CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
        //                            CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
        //                            CreateAt = createCampaignRequest.Campaign!.CreateAt,
        //                            Description = createCampaignRequest.Campaign!.Description,
        //                            DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
        //                            ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
        //                            Image = createCampaignRequest.Campaign!.Image,
        //                            IsActive = createCampaignRequest.Campaign!.IsActive,
        //                            IsComplete = createCampaignRequest.Campaign!.IsComplete,
        //                            IsModify = createCampaignRequest.Campaign!.IsModify,
        //                            IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
        //                            Name = createCampaignRequest.Campaign!.Name,
        //                            Note = createCampaignRequest.Campaign!.Note,
        //                            Organization = createCampaignRequest.Campaign!.Organization,
        //                            ProcessingPhases = createCampaignRequest.Campaign!.ProcessingPhases,
        //                            StartDate = createCampaignRequest.Campaign!.StartDate,
        //                            StatementPhase = statementPhase,
        //                            TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
        //                            Transactions = createCampaignRequest.Campaign!.Transactions,
        //                            UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
        //                            Member = member,
        //                            CampaignTier = createCampaignRequest.Campaign!.CampaignTier
        //                        });
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processing in campaign.ProcessingPhases)
        //                    {
        //                        processing.Activities = null;
        //                        processing.Campaign = null;
        //                        processing.ProcessingPhaseStatementFiles = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //            }
        //            return campaigns.Where(a => a.Name.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedCampaignName));
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("Giai đoạn này không hợp lệ!");
        //        }
        //    }
        //    else
        //    {
        //        if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("donate-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId);
        //            var campaigns = new List<CampaignResponse>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    var member = _userRepository.GetById(userId);
        //                    var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
        //                    if (donatePhase != null && donatePhase.IsProcessing == true)
        //                    {
        //                        createCampaignRequest.Campaign!.DonatePhase = donatePhase;
        //                        campaigns.Add(new CampaignResponse
        //                        {
        //                            CampaignID = createCampaignRequest.Campaign!.CampaignID,
        //                            OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
        //                            CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
        //                            ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
        //                            Address = createCampaignRequest.Campaign!.Address,
        //                            ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
        //                            CampaignType = createCampaignRequest.Campaign!.CampaignType,
        //                            CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
        //                            CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
        //                            CreateAt = createCampaignRequest.Campaign!.CreateAt,
        //                            Description = createCampaignRequest.Campaign!.Description,
        //                            DonatePhase = donatePhase,
        //                            ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
        //                            Image = createCampaignRequest.Campaign!.Image,
        //                            IsActive = createCampaignRequest.Campaign!.IsActive,
        //                            IsComplete = createCampaignRequest.Campaign!.IsComplete,
        //                            IsModify = createCampaignRequest.Campaign!.IsModify,
        //                            IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
        //                            Name = createCampaignRequest.Campaign!.Name,
        //                            Note = createCampaignRequest.Campaign!.Note,
        //                            Organization = createCampaignRequest.Campaign!.Organization,
        //                            ProcessingPhases = createCampaignRequest.Campaign!.ProcessingPhases,
        //                            StartDate = createCampaignRequest.Campaign!.StartDate,
        //                            StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
        //                            TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
        //                            Transactions = createCampaignRequest.Campaign!.Transactions,
        //                            UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
        //                            Member = member,
        //                            CampaignTier = createCampaignRequest.Campaign!.CampaignTier
        //                        });
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processing in campaign.ProcessingPhases)
        //                    {
        //                        processing.Activities = null;
        //                        processing.Campaign = null;
        //                        processing.ProcessingPhaseStatementFiles = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //            }
        //            return campaigns;
        //        }
        //        else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("processing-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId);
        //            var campaigns = new List<CampaignResponse>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    var member = _userRepository.GetById(userId);
        //                    var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
        //                    if (processingPhase != null && processingPhase.Any(pp => pp.IsProcessing) == true)
        //                    {
        //                        campaigns.Add(new CampaignResponse
        //                        {
        //                            CampaignID = createCampaignRequest.Campaign!.CampaignID,
        //                            OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
        //                            CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
        //                            ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
        //                            Address = createCampaignRequest.Campaign!.Address,
        //                            ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
        //                            CampaignType = createCampaignRequest.Campaign!.CampaignType,
        //                            CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
        //                            CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
        //                            CreateAt = createCampaignRequest.Campaign!.CreateAt,
        //                            Description = createCampaignRequest.Campaign!.Description,
        //                            DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
        //                            ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
        //                            Image = createCampaignRequest.Campaign!.Image,
        //                            IsActive = createCampaignRequest.Campaign!.IsActive,
        //                            IsComplete = createCampaignRequest.Campaign!.IsComplete,
        //                            IsModify = createCampaignRequest.Campaign!.IsModify,
        //                            IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
        //                            Name = createCampaignRequest.Campaign!.Name,
        //                            Note = createCampaignRequest.Campaign!.Note,
        //                            Organization = createCampaignRequest.Campaign!.Organization,
        //                            ProcessingPhases = processingPhase,
        //                            StartDate = createCampaignRequest.Campaign!.StartDate,
        //                            StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
        //                            TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
        //                            Transactions = createCampaignRequest.Campaign!.Transactions,
        //                            UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
        //                            Member = member,
        //                            CampaignTier = createCampaignRequest.Campaign!.CampaignTier
        //                        });
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processing in campaign.ProcessingPhases)
        //                    {
        //                        processing.Activities = null;
        //                        processing.Campaign = null;
        //                        processing.ProcessingPhaseStatementFiles = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //            }
        //            return campaigns;
        //        }
        //        else if (!String.IsNullOrEmpty(status) && status.ToLower().Equals("statement-phase".ToLower()))
        //        {
        //            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId);
        //            var campaigns = new List<CampaignResponse>();
        //            if (createCampaignRequests != null)
        //            {
        //                foreach (var createCampaignRequest in createCampaignRequests)
        //                {
        //                    var member = _userRepository.GetById(userId);
        //                    var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
        //                    if (statementPhase != null && statementPhase.IsProcessing == true)
        //                    {
        //                        campaigns.Add(new CampaignResponse
        //                        {
        //                            CampaignID = createCampaignRequest.Campaign!.CampaignID,
        //                            OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
        //                            CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
        //                            ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
        //                            Address = createCampaignRequest.Campaign!.Address,
        //                            ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
        //                            CampaignType = createCampaignRequest.Campaign!.CampaignType,
        //                            CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
        //                            CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
        //                            CreateAt = createCampaignRequest.Campaign!.CreateAt,
        //                            Description = createCampaignRequest.Campaign!.Description,
        //                            DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
        //                            ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
        //                            Image = createCampaignRequest.Campaign!.Image,
        //                            IsActive = createCampaignRequest.Campaign!.IsActive,
        //                            IsComplete = createCampaignRequest.Campaign!.IsComplete,
        //                            IsModify = createCampaignRequest.Campaign!.IsModify,
        //                            IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
        //                            Name = createCampaignRequest.Campaign!.Name,
        //                            Note = createCampaignRequest.Campaign!.Note,
        //                            Organization = createCampaignRequest.Campaign!.Organization,
        //                            ProcessingPhases = createCampaignRequest.Campaign!.ProcessingPhases,
        //                            StartDate = createCampaignRequest.Campaign!.StartDate,
        //                            StatementPhase = statementPhase,
        //                            TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
        //                            Transactions = createCampaignRequest.Campaign!.Transactions,
        //                            UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
        //                            Member = member,
        //                            CampaignTier = createCampaignRequest.Campaign!.CampaignTier
        //                        });
        //                    }
        //                }
        //            }

        //            foreach (var campaign in campaigns)
        //            {
        //                if (campaign.CampaignType != null)
        //                    campaign.CampaignType!.Campaigns = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization.OrganizationManager = null;
        //                if (campaign.Organization != null)
        //                    campaign.Organization!.Campaigns = null;
        //                if (campaign.DonatePhase != null)
        //                    campaign.DonatePhase.Campaign = null;
        //                if (campaign.ProcessingPhases != null)
        //                    foreach (var processing in campaign.ProcessingPhases)
        //                    {
        //                        processing.Activities = null;
        //                        processing.Campaign = null;
        //                        processing.ProcessingPhaseStatementFiles = null;
        //                    }
        //                if (campaign.StatementPhase != null)
        //                {
        //                    campaign.StatementPhase.Campaign = null;
        //                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //                    foreach (var statementFile in statementFiles)
        //                    {
        //                        statementFile.StatementPhase = null;
        //                    }
        //                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //                }
        //            }
        //            return campaigns;
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("Giai đoạn này không hợp lệ!");
        //        }
        //    }
        //}

        public CampaignCreateByOm? GetAllCampaignByCreateByOrganizationManagerIdWithOptionsPhaseInProcessingPhase(
            Guid organizationManagerId, string? status, string? campaignName, int? pageSize, int? pageNo)
        {
            // Normalize campaign name once
            string? normalizedCampaignName = campaignName?.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            bool hasCampaignName = !string.IsNullOrEmpty(normalizedCampaignName);

            // Normalize status once
            string? normalizedStatus = status?.ToLowerInvariant();

            // Fetch createCampaignRequests once
            var createCampaignRequests =
                _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(
                    organizationManagerId, pageSize, pageNo);

            if (createCampaignRequests == null || !createCampaignRequests.Any())
                return null;

            var campaigns = new List<Campaign>();

            foreach (var createCampaignRequest in createCampaignRequests)
            {
                var campaign = createCampaignRequest.Campaign;
                if (campaign == null || campaign.OrganizationID == null)
                    continue;

                // Load organization and phases as required
                campaign.Organization = _organizationRepository.GetById((Guid)campaign.OrganizationID);

                if (normalizedStatus == "donate-phase")
                {
                    campaign.DonatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID);
                    if (campaign.DonatePhase?.IsProcessing == true && campaign.CampaignTier == CampaignTier.FullDisbursementCampaign)
                        campaigns.Add(campaign);
                }
                else if (normalizedStatus == "processing-phase")
                {
                    var processingPhases =
                        _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaign.CampaignID);
                    if (processingPhases?.Any(pp => pp.IsProcessing) == true && campaign.CampaignTier == CampaignTier.FullDisbursementCampaign)
                    {
                        campaign.ProcessingPhases = processingPhases;
                        campaigns.Add(campaign);
                    }
                }
                else if (normalizedStatus == "statement-phase")
                {
                    campaign.StatementPhase =
                        _statementPhaseRepository.GetStatementPhaseByCampaignId(campaign.CampaignID);
                    if (campaign.StatementPhase?.IsProcessing == true && campaign.CampaignTier == CampaignTier.FullDisbursementCampaign)
                    {
                        campaign.StatementPhase.StatementFiles = _statementFileRepository.GetAll()
                            .Where(s => s.StatementPhaseId == campaign.StatementPhase.StatementPhaseId).ToList();
                        campaigns.Add(campaign);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Giai đoạn này không hợp lệ!");
                }

                // Detach unnecessary navigation properties
                DetachNavigationProperties(campaign);
            }

            if (hasCampaignName)
            {
                return new CampaignCreateByOm
                {
                    Campaigns = campaigns.Where(c =>
                            c.Name.ToLowerInvariant().Normalize(NormalizationForm.FormD)
                                .Contains(normalizedCampaignName))
                        .ToList(),
                    TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId, status).Count()
                };
            }
            return new CampaignCreateByOm
            {
                Campaigns = campaigns,
                TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId, status).Count()
            };
        }



        private void DetachNavigationProperties(Campaign campaign)
        {
            if (campaign.CampaignType != null)
                campaign.CampaignType.Campaigns = null;

            if (campaign.Organization != null)
            {
                campaign.Organization.OrganizationManager = null;
                campaign.Organization.Campaigns = null;
            }

            if (campaign.DonatePhase != null)
                campaign.DonatePhase.Campaign = null;

            if (campaign.ProcessingPhases != null)
            {
                foreach (var processingPhase in campaign.ProcessingPhases)
                    processingPhase.Campaign = null;
            }

            if (campaign.StatementPhase != null)
                campaign.StatementPhase.Campaign = null;

            if (campaign.CreateCampaignRequest != null)
            {
                campaign.CreateCampaignRequest.OrganizationManager = null;
                campaign.CreateCampaignRequest.Member = null;
                campaign.CreateCampaignRequest.Moderator = null;
            }
        }


        public CampaignCreateByVolunteer? GetAllCampaignByCreateByVolunteerIdWithOptionsPhaseInProcessingPhase(Guid userId, string? status, string? campaignName, int? pageSize, int? pageNo)
        {
            if (string.IsNullOrEmpty(campaignName) && string.IsNullOrEmpty(status))
            {
                throw new InvalidOperationException("Giai đoạn này không hợp lệ!");
            }

            string? normalizedCampaignName = null;
            if (!string.IsNullOrEmpty(campaignName))
            {
                normalizedCampaignName = campaignName.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            }

            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignTierIRequestByVolunteerId(userId, pageSize, pageNo);

            if (createCampaignRequests == null) return null;

            var campaigns = new List<CampaignResponse>();

            foreach (var createCampaignRequest in createCampaignRequests)
            {
                var member = _userRepository.GetById(userId);
                var phases = GetPhases(createCampaignRequest.CampaignID, status);

                if (phases == null || !phases.Any()) continue;

                var campaignResponse = MapToCampaignResponse(createCampaignRequest, member, phases);
                campaigns.Add(campaignResponse);
            }

            foreach (var campaign in campaigns)
            {
                ClearUnnecessaryReferences(campaign);
            }

            if (normalizedCampaignName != null)
            {
                campaigns = campaigns
                    .Where(c => c.Name.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedCampaignName))
                    .ToList();
            }

            return new CampaignCreateByVolunteer
            {
                Campaigns = campaigns,
                TotalItem = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId, status).Count()
            };
        }

        private IEnumerable<object>? GetPhases(Guid campaignId, string? status)
        {
            return status?.ToLower() switch
            {
                "donate-phase" => _donatePhaseRepository.GetDonatePhaseByCampaignId(campaignId)?.IsProcessing == true ?
                                  new List<object> { _donatePhaseRepository.GetDonatePhaseByCampaignId(campaignId) } : null,

                "processing-phase" => _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaignId)?
                                      .Where(pp => pp.IsProcessing).ToList(),

                "statement-phase" => _statementPhaseRepository.GetStatementPhaseByCampaignId(campaignId)?.IsProcessing == true ?
                                     new List<object> { _statementPhaseRepository.GetStatementPhaseByCampaignId(campaignId) } : null,

                _ => throw new InvalidOperationException("Giai đoạn này không hợp lệ!")
            };
        }

        private CampaignResponse MapToCampaignResponse(CreateCampaignRequest createCampaignRequest, Member member, IEnumerable<object> phases)
        {
            var campaign = createCampaignRequest.Campaign!;
            var response = new CampaignResponse
            {
                CampaignID = campaign.CampaignID,
                OrganizationID = campaign.OrganizationID,
                CampaignTypeID = campaign.CampaignTypeID,
                ActualEndDate = campaign.ActualEndDate,
                Address = campaign.Address,
                ApplicationConfirmForm = campaign.ApplicationConfirmForm,
                CampaignType = campaign.CampaignType,
                CanBeDonated = campaign.CanBeDonated,
                CheckTransparentDate = campaign.CheckTransparentDate,
                CreateAt = campaign.CreateAt,
                Description = campaign.Description,
                DonatePhase = phases.OfType<DonatePhase>().FirstOrDefault(),
                ExpectedEndDate = campaign.ExpectedEndDate,
                Image = campaign.Image,
                IsActive = campaign.IsActive,
                IsComplete = campaign.IsComplete,
                IsModify = campaign.IsModify,
                IsTransparent = campaign.IsTransparent,
                Name = campaign.Name,
                Note = campaign.Note,
                Organization = campaign.Organization,
                ProcessingPhases = phases.OfType<ProcessingPhase>().ToList(),
                StartDate = campaign.StartDate,
                StatementPhase = phases.OfType<StatementPhase>().FirstOrDefault(),
                TargetAmount = campaign.TargetAmount,
                Transactions = campaign.Transactions,
                UpdatedAt = campaign.UpdatedAt,
                Member = member,
                CampaignTier = campaign.CampaignTier
            };

            if (response.StatementPhase != null)
            {
                var statementFiles = _statementFileRepository.GetAll()
                    .Where(s => s.StatementPhaseId.Equals(response.StatementPhase.StatementPhaseId)).ToList();
                foreach (var statementFile in statementFiles)
                {
                    statementFile.StatementPhase = null;
                }
                response.StatementPhase.StatementFiles = statementFiles;
            }

            return response;
        }

        private void ClearUnnecessaryReferences(CampaignResponse campaign)
        {
            if (campaign.CampaignType != null)
                campaign.CampaignType!.Campaigns = null;
            if (campaign.Organization != null)
                campaign.Organization.OrganizationManager = null;
            if (campaign.Organization != null)
                campaign.Organization!.Campaigns = null;
            if (campaign.DonatePhase != null)
                campaign.DonatePhase.Campaign = null;
            if (campaign.ProcessingPhases != null)
            {
                foreach (var processing in campaign.ProcessingPhases)
                {
                    processing.Activities = null;
                    processing.Campaign = null;
                    processing.ProcessingPhaseStatementFiles = null;
                }
            }
            if (campaign.StatementPhase != null)
            {
                campaign.StatementPhase.Campaign = null;
            }
        }



        //public IEnumerable<CampaignResponse?> GetAllCampaignByCreateByVolunteerIdWithDonatePhaseIsProcessing(Guid userId)
        //{
        //    var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId);
        //    var campaigns = new List<CampaignResponse>();
        //    if (createCampaignRequests != null)
        //    {
        //        foreach (var createCampaignRequest in createCampaignRequests)
        //        {
        //            var member = _userRepository.GetById(userId);
        //            var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(createCampaignRequest.CampaignID);
        //            if (donatePhase != null && donatePhase.IsProcessing == true)
        //            {
        //                campaigns.Add(new CampaignResponse
        //                {
        //                    CampaignID = createCampaignRequest.Campaign!.CampaignID,
        //                    OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
        //                    CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
        //                    ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
        //                    Address = createCampaignRequest.Campaign!.Address,
        //                    ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
        //                    CampaignType = createCampaignRequest.Campaign!.CampaignType,
        //                    CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
        //                    CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
        //                    CreateAt = createCampaignRequest.Campaign!.CreateAt,
        //                    Description = createCampaignRequest.Campaign!.Description,
        //                    DonatePhase = donatePhase,
        //                    ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
        //                    Image = createCampaignRequest.Campaign!.Image,
        //                    IsActive = createCampaignRequest.Campaign!.IsActive,
        //                    IsComplete = createCampaignRequest.Campaign!.IsComplete,
        //                    IsModify = createCampaignRequest.Campaign!.IsModify,
        //                    IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
        //                    Name = createCampaignRequest.Campaign!.Name,
        //                    Note = createCampaignRequest.Campaign!.Note,
        //                    Organization = createCampaignRequest.Campaign!.Organization,
        //                    ProcessingPhases = createCampaignRequest.Campaign!.ProcessingPhases,
        //                    StartDate = createCampaignRequest.Campaign!.StartDate,
        //                    StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
        //                    TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
        //                    Transactions = createCampaignRequest.Campaign!.Transactions,
        //                    UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
        //                    Member = member,
        //                    CampaignTier = createCampaignRequest.Campaign!.CampaignTier
        //                });
        //            }
        //        }
        //    }

        //    foreach (var campaign in campaigns)
        //    {
        //        if (campaign.CampaignType != null)
        //            campaign.CampaignType!.Campaigns = null;
        //        if (campaign.Organization != null)
        //            campaign.Organization.OrganizationManager = null;
        //        if (campaign.Organization != null)
        //            campaign.Organization!.Campaigns = null;
        //        if (campaign.ProcessingPhases != null)
        //            foreach (var processing in campaign.ProcessingPhases)
        //            {
        //                processing.Activities = null;
        //                processing.Campaign = null;
        //                processing.ProcessingPhaseStatementFiles = null;
        //            }
        //        if (campaign.StatementPhase != null)
        //        {
        //            campaign.StatementPhase.Campaign = null;
        //            var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //            foreach (var statementFile in statementFiles)
        //            {
        //                statementFile.StatementPhase = null;
        //            }
        //            campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //        }
        //    }
        //    return campaigns;
        //}


        public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerIdWithProcessingPhaseIsProcessing(Guid organizationManagerId, int? pageSize, int? pageNo)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId, pageSize, pageNo);
            var campaigns = new List<Campaign>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                    {
                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                        createCampaignRequest.Campaign!.Organization = organization;
                        var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
                        if (processingPhase != null && processingPhase.Any(pp => pp.IsProcessing) == true)
                        {
                            createCampaignRequest.Campaign!.Organization = organization;
                            createCampaignRequest.Campaign!.ProcessingPhases = processingPhase;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.DonatePhase != null)
                    campaign.DonatePhase.Campaign = null;
                if (campaign.ProcessingPhases != null)
                    foreach (var processing in campaign.ProcessingPhases)
                    {
                        processing.Activities = null;
                        processing.Campaign = null;
                        processing.ProcessingPhaseStatementFiles = null;
                    }
                if (campaign.StatementPhase != null)
                {
                    campaign.StatementPhase.Campaign = null;
                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                    foreach (var statementFile in statementFiles)
                    {
                        statementFile.StatementPhase = null;
                    }
                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                }
                if (campaign.CreateCampaignRequest != null)
                {
                    campaign.CreateCampaignRequest.OrganizationManager = null;
                    campaign.CreateCampaignRequest.Member = null;
                    campaign.CreateCampaignRequest.Moderator = null;
                }
            }
            return campaigns;
        }


        //public IEnumerable<CampaignResponse?> GetAllCampaignByCreateByVolunteerIdWithProcessingPhaseIsProcessing(Guid userId)
        //{
        //    var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId);
        //    var campaigns = new List<CampaignResponse>();
        //    if (createCampaignRequests != null)
        //    {
        //        foreach (var createCampaignRequest in createCampaignRequests)
        //        {
        //            var member = _userRepository.GetById(userId);
        //            var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(createCampaignRequest.CampaignID);
        //            if (processingPhase != null && processingPhase.Any(pp => pp.IsProcessing) == true)
        //            {
        //                campaigns.Add(new CampaignResponse
        //                {
        //                    CampaignID = createCampaignRequest.Campaign!.CampaignID,
        //                    OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
        //                    CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
        //                    ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
        //                    Address = createCampaignRequest.Campaign!.Address,
        //                    ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
        //                    CampaignType = createCampaignRequest.Campaign!.CampaignType,
        //                    CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
        //                    CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
        //                    CreateAt = createCampaignRequest.Campaign!.CreateAt,
        //                    Description = createCampaignRequest.Campaign!.Description,
        //                    DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
        //                    ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
        //                    Image = createCampaignRequest.Campaign!.Image,
        //                    IsActive = createCampaignRequest.Campaign!.IsActive,
        //                    IsComplete = createCampaignRequest.Campaign!.IsComplete,
        //                    IsModify = createCampaignRequest.Campaign!.IsModify,
        //                    IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
        //                    Name = createCampaignRequest.Campaign!.Name,
        //                    Note = createCampaignRequest.Campaign!.Note,
        //                    Organization = createCampaignRequest.Campaign!.Organization,
        //                    ProcessingPhases = processingPhase,
        //                    StartDate = createCampaignRequest.Campaign!.StartDate,
        //                    StatementPhase = createCampaignRequest.Campaign!.StatementPhase,
        //                    TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
        //                    Transactions = createCampaignRequest.Campaign!.Transactions,
        //                    UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
        //                    Member = member,
        //                    CampaignTier = createCampaignRequest.Campaign!.CampaignTier
        //                });
        //            }
        //        }
        //    }

        //    foreach (var campaign in campaigns)
        //    {
        //        if (campaign.CampaignType != null)
        //            campaign.CampaignType!.Campaigns = null;
        //        if (campaign.Organization != null)
        //            campaign.Organization.OrganizationManager = null;
        //        if (campaign.Organization != null)
        //            campaign.Organization!.Campaigns = null;
        //        if (campaign.DonatePhase != null)
        //            campaign.DonatePhase.Campaign = null;
        //        if (campaign.ProcessingPhases != null)
        //            foreach (var processing in campaign.ProcessingPhases)
        //            {
        //                processing.Activities = null;
        //                processing.Campaign = null;
        //                processing.ProcessingPhaseStatementFiles = null;
        //            }
        //        if (campaign.StatementPhase != null)
        //        {
        //            campaign.StatementPhase.Campaign = null;
        //            var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //            foreach (var statementFile in statementFiles)
        //            {
        //                statementFile.StatementPhase = null;
        //            }
        //            campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //        }
        //    }
        //    return campaigns;
        //}




        public IEnumerable<Campaign?> GetAllCampaignByCreateByOrganizationManagerIdWithStatementIsProcessing(Guid organizationManagerId, int? pageSize, int? pageNo)
        {
            var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByOrganizationManagerId(organizationManagerId, pageSize, pageNo);
            var campaigns = new List<Campaign>();
            if (createCampaignRequests != null)
            {
                foreach (var createCampaignRequest in createCampaignRequests)
                {
                    if (createCampaignRequest.Campaign != null && createCampaignRequest.Campaign.OrganizationID != null)
                    {
                        var organization = _organizationRepository.GetById((Guid)createCampaignRequest.Campaign!.OrganizationID!);
                        createCampaignRequest.Campaign!.Organization = organization;
                        var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
                        if (statementPhase != null && statementPhase.IsProcessing == true)
                        {
                            createCampaignRequest.Campaign!.Organization = organization;
                            createCampaignRequest.Campaign!.StatementPhase = statementPhase;
                            campaigns.Add(createCampaignRequest.Campaign!);
                        }
                    }
                }
            }

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignType != null)
                    campaign.CampaignType!.Campaigns = null;
                if (campaign.Organization != null)
                    campaign.Organization.OrganizationManager = null;
                if (campaign.Organization != null)
                    campaign.Organization!.Campaigns = null;
                if (campaign.DonatePhase != null)
                    campaign.DonatePhase.Campaign = null;
                if (campaign.ProcessingPhases != null)
                    foreach (var processingPhase in campaign.ProcessingPhases)
                    {
                        processingPhase.Campaign = null;
                    }
                if (campaign.StatementPhase != null)
                {
                    campaign.StatementPhase.Campaign = null;
                    var statementFiles = _statementFileRepository.GetAll().Where(s =>
                        s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
                    foreach (var statementFile in statementFiles)
                    {
                        statementFile.StatementPhase = null;
                    }
                    campaign.StatementPhase.StatementFiles = statementFiles.ToList();
                }
                if (campaign.CreateCampaignRequest != null)
                {
                    campaign.CreateCampaignRequest.OrganizationManager = null;
                    campaign.CreateCampaignRequest.Member = null;
                    campaign.CreateCampaignRequest.Moderator = null;
                }
            }
            return campaigns;
        }



        //public IEnumerable<CampaignResponse?> GetAllCampaignByCreateByVolunteerIdWithStatementPhaseIsProcessing(Guid userId)
        //{
        //    var createCampaignRequests = _createCampaignRequestRepository.GetAllCreateCampaignRequestByVolunteerId(userId);
        //    var campaigns = new List<CampaignResponse>();
        //    if (createCampaignRequests != null)
        //    {
        //        foreach (var createCampaignRequest in createCampaignRequests)
        //        {
        //            var member = _userRepository.GetById(userId);
        //            var statementPhase = _statementPhaseRepository.GetStatementPhaseByCampaignId(createCampaignRequest.CampaignID);
        //            if (statementPhase != null && statementPhase.IsProcessing == true)
        //            {
        //                campaigns.Add(new CampaignResponse
        //                {
        //                    CampaignID = createCampaignRequest.Campaign!.CampaignID,
        //                    OrganizationID = createCampaignRequest.Campaign!.OrganizationID,
        //                    CampaignTypeID = createCampaignRequest.Campaign!.CampaignTypeID,
        //                    ActualEndDate = createCampaignRequest.Campaign!.ActualEndDate,
        //                    Address = createCampaignRequest.Campaign!.Address,
        //                    ApplicationConfirmForm = createCampaignRequest.Campaign!.ApplicationConfirmForm,
        //                    CampaignType = createCampaignRequest.Campaign!.CampaignType,
        //                    CanBeDonated = createCampaignRequest.Campaign!.CanBeDonated,
        //                    CheckTransparentDate = createCampaignRequest.Campaign!.CheckTransparentDate,
        //                    CreateAt = createCampaignRequest.Campaign!.CreateAt,
        //                    Description = createCampaignRequest.Campaign!.Description,
        //                    DonatePhase = createCampaignRequest.Campaign!.DonatePhase,
        //                    ExpectedEndDate = createCampaignRequest.Campaign!.ExpectedEndDate,
        //                    Image = createCampaignRequest.Campaign!.Image,
        //                    IsActive = createCampaignRequest.Campaign!.IsActive,
        //                    IsComplete = createCampaignRequest.Campaign!.IsComplete,
        //                    IsModify = createCampaignRequest.Campaign!.IsModify,
        //                    IsTransparent = createCampaignRequest.Campaign!.IsTransparent,
        //                    Name = createCampaignRequest.Campaign!.Name,
        //                    Note = createCampaignRequest.Campaign!.Note,
        //                    Organization = createCampaignRequest.Campaign!.Organization,
        //                    ProcessingPhases = createCampaignRequest.Campaign!.ProcessingPhases,
        //                    StartDate = createCampaignRequest.Campaign!.StartDate,
        //                    StatementPhase = statementPhase,
        //                    TargetAmount = createCampaignRequest.Campaign!.TargetAmount,
        //                    Transactions = createCampaignRequest.Campaign!.Transactions,
        //                    UpdatedAt = createCampaignRequest.Campaign!.UpdatedAt,
        //                    Member = member,
        //                    CampaignTier = createCampaignRequest.Campaign!.CampaignTier
        //                });
        //            }
        //        }
        //    }

        //    foreach (var campaign in campaigns)
        //    {
        //        if (campaign.CampaignType != null)
        //            campaign.CampaignType!.Campaigns = null;
        //        if (campaign.Organization != null)
        //            campaign.Organization.OrganizationManager = null;
        //        if (campaign.Organization != null)
        //            campaign.Organization!.Campaigns = null;
        //        if (campaign.StatementPhase != null)
        //        {
        //            campaign.StatementPhase.Campaign = null;
        //            var statementFiles = _statementFileRepository.GetAll().Where(s =>
        //                s.StatementPhaseId.Equals(campaign.StatementPhase.StatementPhaseId));
        //            foreach (var statementFile in statementFiles)
        //            {
        //                statementFile.StatementPhase = null;
        //            }
        //            campaign.StatementPhase.StatementFiles = statementFiles.ToList();
        //        }
        //    }
        //    return campaigns;
        //}


        public IEnumerable<CampaignResponse?> GetAllCampaignResponsesByCampaignName(string? campaignName)
        {
            var campaignResponses = new List<CampaignResponse>();
            if (!String.IsNullOrEmpty(campaignName))
            {
                var campaigns = GetAllCampaigns().Where(c => c.Name.ToLower().Contains(campaignName.ToLower().Trim()));
                foreach (var campaign in campaigns)
                {
                    var createcampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (createcampaignRequest != null && createcampaignRequest.CreateByMember != null)
                    {
                        var member = _userRepository.GetById((Guid)createcampaignRequest.CreateByMember);

                        campaignResponses.Add(new CampaignResponse
                        {
                            CampaignID = campaign.CampaignID,
                            OrganizationID = campaign.OrganizationID,
                            CampaignTypeID = campaign.CampaignTypeID,
                            ActualEndDate = campaign.ActualEndDate,
                            Address = campaign.Address,
                            ApplicationConfirmForm = campaign.ApplicationConfirmForm,
                            CampaignType = campaign.CampaignType,
                            CanBeDonated = campaign.CanBeDonated,
                            CheckTransparentDate = campaign.CheckTransparentDate,
                            CreateAt = campaign.CreateAt,
                            Description = campaign.Description,
                            DonatePhase = campaign.DonatePhase,
                            ExpectedEndDate = campaign.ExpectedEndDate,
                            Image = campaign.Image,
                            IsActive = campaign.IsActive,
                            IsComplete = campaign.IsComplete,
                            IsModify = campaign.IsModify,
                            IsTransparent = campaign.IsTransparent,
                            Name = campaign.Name,
                            Note = campaign.Note,
                            Organization = campaign.Organization,
                            ProcessingPhases = campaign.ProcessingPhases,
                            StartDate = campaign.StartDate,
                            StatementPhase = campaign.StatementPhase,
                            TargetAmount = campaign.TargetAmount,
                            Transactions = campaign.Transactions,
                            UpdatedAt = campaign.UpdatedAt,
                            Member = member
                        });
                    }

                    if (createcampaignRequest != null && createcampaignRequest.CreateByOM != null)
                    {
                        var om = _organizationManagerRepository.GetById((Guid)createcampaignRequest.CreateByOM);

                        campaignResponses.Add(new CampaignResponse
                        {
                            CampaignID = campaign.CampaignID,
                            OrganizationID = campaign.OrganizationID,
                            CampaignTypeID = campaign.CampaignTypeID,
                            ActualEndDate = campaign.ActualEndDate,
                            Address = campaign.Address,
                            ApplicationConfirmForm = campaign.ApplicationConfirmForm,
                            CampaignType = campaign.CampaignType,
                            CanBeDonated = campaign.CanBeDonated,
                            CheckTransparentDate = campaign.CheckTransparentDate,
                            CreateAt = campaign.CreateAt,
                            Description = campaign.Description,
                            DonatePhase = campaign.DonatePhase,
                            ExpectedEndDate = campaign.ExpectedEndDate,
                            Image = campaign.Image,
                            IsActive = campaign.IsActive,
                            IsComplete = campaign.IsComplete,
                            IsModify = campaign.IsModify,
                            IsTransparent = campaign.IsTransparent,
                            Name = campaign.Name,
                            Note = campaign.Note,
                            Organization = campaign.Organization,
                            ProcessingPhases = campaign.ProcessingPhases,
                            StartDate = campaign.StartDate,
                            StatementPhase = campaign.StatementPhase,
                            TargetAmount = campaign.TargetAmount,
                            Transactions = campaign.Transactions,
                            UpdatedAt = campaign.UpdatedAt,
                            OrganizationManager = om
                        });
                    }

                }

                foreach (var campaignResponse in campaignResponses)
                {
                    if (campaignResponse.CampaignType != null)
                        campaignResponse.CampaignType!.Campaigns = null;
                    if (campaignResponse.Organization != null)
                        campaignResponse.Organization.OrganizationManager = null;
                    if (campaignResponse.Organization != null)
                        campaignResponse.Organization!.Campaigns = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.Organizations = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.BankingAccounts = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.CreateOrganizationRequests = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.CreateCampaignRequests = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.CreateActivityRequests = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.CreatePostRequests = null;
                    if (campaignResponse.DonatePhase != null)
                        campaignResponse.DonatePhase!.Campaign = null;
                    if (campaignResponse.Transactions != null)
                        campaignResponse.Transactions = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.CreateMemberVerifiedRequests = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.BankingAccounts = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.CreateCampaignRequests = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.CreateActivityRequests = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.CreatePostRequests = null;
                }
                return campaignResponses;
            }
            else
            {
                var campaigns = GetAllCampaigns();
                foreach (var campaign in campaigns)
                {
                    var createcampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID);
                    if (createcampaignRequest != null && createcampaignRequest.CreateByMember != null)
                    {
                        var member = _userRepository.GetById((Guid)createcampaignRequest.CreateByMember);

                        campaignResponses.Add(new CampaignResponse
                        {
                            CampaignID = campaign.CampaignID,
                            OrganizationID = campaign.OrganizationID,
                            CampaignTypeID = campaign.CampaignTypeID,
                            ActualEndDate = campaign.ActualEndDate,
                            Address = campaign.Address,
                            ApplicationConfirmForm = campaign.ApplicationConfirmForm,
                            CampaignType = campaign.CampaignType,
                            CanBeDonated = campaign.CanBeDonated,
                            CheckTransparentDate = campaign.CheckTransparentDate,
                            CreateAt = campaign.CreateAt,
                            Description = campaign.Description,
                            DonatePhase = campaign.DonatePhase,
                            ExpectedEndDate = campaign.ExpectedEndDate,
                            Image = campaign.Image,
                            IsActive = campaign.IsActive,
                            IsComplete = campaign.IsComplete,
                            IsModify = campaign.IsModify,
                            IsTransparent = campaign.IsTransparent,
                            Name = campaign.Name,
                            Note = campaign.Note,
                            Organization = campaign.Organization,
                            ProcessingPhases = campaign.ProcessingPhases,
                            StartDate = campaign.StartDate,
                            StatementPhase = campaign.StatementPhase,
                            TargetAmount = campaign.TargetAmount,
                            Transactions = campaign.Transactions,
                            UpdatedAt = campaign.UpdatedAt,
                            Member = member
                        });
                    }

                    if (createcampaignRequest != null && createcampaignRequest.CreateByOM != null)
                    {
                        var om = _organizationManagerRepository.GetById((Guid)createcampaignRequest.CreateByOM);

                        campaignResponses.Add(new CampaignResponse
                        {
                            CampaignID = campaign.CampaignID,
                            OrganizationID = campaign.OrganizationID,
                            CampaignTypeID = campaign.CampaignTypeID,
                            ActualEndDate = campaign.ActualEndDate,
                            Address = campaign.Address,
                            ApplicationConfirmForm = campaign.ApplicationConfirmForm,
                            CampaignType = campaign.CampaignType,
                            CanBeDonated = campaign.CanBeDonated,
                            CheckTransparentDate = campaign.CheckTransparentDate,
                            CreateAt = campaign.CreateAt,
                            Description = campaign.Description,
                            DonatePhase = campaign.DonatePhase,
                            ExpectedEndDate = campaign.ExpectedEndDate,
                            Image = campaign.Image,
                            IsActive = campaign.IsActive,
                            IsComplete = campaign.IsComplete,
                            IsModify = campaign.IsModify,
                            IsTransparent = campaign.IsTransparent,
                            Name = campaign.Name,
                            Note = campaign.Note,
                            Organization = campaign.Organization,
                            ProcessingPhases = campaign.ProcessingPhases,
                            StartDate = campaign.StartDate,
                            StatementPhase = campaign.StatementPhase,
                            TargetAmount = campaign.TargetAmount,
                            Transactions = campaign.Transactions,
                            UpdatedAt = campaign.UpdatedAt,
                            OrganizationManager = om
                        });
                    }

                }

                foreach (var campaignResponse in campaignResponses)
                {
                    if (campaignResponse.CampaignType != null)
                        campaignResponse.CampaignType!.Campaigns = null;
                    if (campaignResponse.Organization != null)
                        campaignResponse.Organization.OrganizationManager = null;
                    if (campaignResponse.Organization != null)
                        campaignResponse.Organization!.Campaigns = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.Organizations = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.BankingAccounts = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.CreateOrganizationRequests = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.CreateCampaignRequests = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.CreateActivityRequests = null;
                    if (campaignResponse.OrganizationManager != null)
                        campaignResponse.OrganizationManager!.CreatePostRequests = null;
                    if (campaignResponse.DonatePhase != null)
                        campaignResponse.DonatePhase!.Campaign = null;
                    if (campaignResponse.Transactions != null)
                        campaignResponse.Transactions = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.CreateMemberVerifiedRequests = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.BankingAccounts = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.CreateCampaignRequests = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.CreateActivityRequests = null;
                    if (campaignResponse.Member != null)
                        campaignResponse.Member!.CreatePostRequests = null;
                }
                return campaignResponses;
            }
        }



        public async Task<Campaign?> CreateCampaign(CreateNewCampaignRequest request)
        {
            TryValidateCreateCampaignRequest(request);
            var campaign = new Campaign();
            if (request.OrganizationID == null)
            {
                campaign = new Campaign
                {
                    CampaignID = Guid.NewGuid(),
                    CampaignTypeID = request.CampaignTypeId,
                    Address = request.Address,
                    ApplicationConfirmForm = request.ApplicationConfirmForm,
                    Name = request.Name,
                    Description = request.Description,
                    Image = await _firebaseService.UploadImage(request.ImageCampaign),
                    StartDate = request.StartDate,
                    ExpectedEndDate = request.ExpectedEndDate,
                    TargetAmount = request.TargetAmount,
                    IsTransparent = false,
                    CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                    IsActive = false,
                    IsModify = false,
                    IsComplete = false,
                    CanBeDonated = false,
                };
                return _campaignRepository.Save(campaign);
            }
            else
            {
                campaign = new Campaign
                {
                    CampaignID = Guid.NewGuid(),
                    CampaignTypeID = request.CampaignTypeId,
                    OrganizationID = request.OrganizationID,
                    Address = request.Address,
                    ApplicationConfirmForm = request.ApplicationConfirmForm,
                    Name = request.Name,
                    Description = request.Description,
                    Image = await _firebaseService.UploadImage(request.ImageCampaign),
                    StartDate = request.StartDate,
                    ExpectedEndDate = request.ExpectedEndDate,
                    TargetAmount = request.TargetAmount,
                    IsTransparent = false,
                    CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                    IsActive = false,
                    IsModify = false,
                    IsComplete = false,
                    CanBeDonated = false,
                };
                return _campaignRepository.Save(campaign);
            }
        }


        public void SendEmailforReportCampaign(Guid campaignId)
        {
            var campaign = _campaignRepository.GetById(campaignId);
            if (campaign == null)
            {
                throw new NotFoundException("Chiến dịch này không tồn tại!");
            }

            var listEmail = _accountRepository.GetAll().Select(c => new { c.Email, c.Username }).ToList();
            listEmail.ForEach(item => EmailSupporter.SendEmailForReportCampaign(item.Email, campaign.Name, item.Username, campaign.StartDate, campaign.ExpectedEndDate));
        }


        private void TryValidateCreateCampaignRequest(CreateNewCampaignRequest request)
        {
            if (!String.IsNullOrEmpty(request.CampaignTypeId.ToString()))
            {
                throw new Exception("CampaignTypeId must not empty.");
            }

            if (_campaignTypeRepository.GetById(request.CampaignTypeId) == null)
            {
                throw new Exception("Loại chiến dịch không tìm thấy.");
            }
            if (!String.IsNullOrEmpty(request.Name))
            {
                throw new Exception("Tên chiến dịch không được để trống.");
            }
            if (!String.IsNullOrEmpty(request.Address))
            {
                throw new Exception("Địa chỉ không được để trống.");
            }
            if (!String.IsNullOrEmpty(request.Description))
            {
                throw new Exception("Mô tả không được để trống.");
            }
            if (!String.IsNullOrEmpty(request.StartDate.ToString()))
            {
                throw new Exception("Ngày bắt đầu không được để trống.");
            }
            if (!String.IsNullOrEmpty(request.TargetAmount))
            {
                throw new Exception("Số tiền mục tiêu không được để trống.");
            }
            if (!String.IsNullOrEmpty(request.ApplicationConfirmForm))
            {
                throw new Exception("Đơn xác nhận không được để trống.");
            }
            if (!String.IsNullOrEmpty(request.AccountName))
            {
                throw new Exception("Tên tài khoản không được để trống.");
            }
            if (request.StartDate < TimeHelper.GetTime(DateTime.UtcNow))
            {
                throw new Exception("Ngày bắt đầu phải lớn hơn ngày hiện tại!");
            }

            if (request.ExpectedEndDate < TimeHelper.GetTime(DateTime.UtcNow))
            {
                throw new Exception("Ngày kết thúc phải lớn hơn ngày hiện tại!");
            }

            if (request.StartDate > request.ExpectedEndDate)
            {
                throw new Exception("Ngày kết thúc phải lớn hơn ngày bắt đầu!");
            }

            if (Convert.ToInt64(request.TargetAmount) < 0)
            {
                throw new Exception("Số tiền mục tiêu phải lớn hơn hoặc bằng 0.");
            }
        }


    }
}
