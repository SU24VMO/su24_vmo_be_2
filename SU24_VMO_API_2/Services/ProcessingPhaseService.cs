﻿using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Response;

namespace SU24_VMO_API.Services
{
    public class ProcessingPhaseService
    {
        private readonly IProcessingPhaseRepository repository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IStatementPhaseRepository _statementRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAccountRepository _accountRepository;

        public ProcessingPhaseService(IProcessingPhaseRepository repository, ICampaignRepository campaignRepository, IMemberRepository memberRepository,
            IOrganizationManagerRepository organizationManagerRepository, ICreateCampaignRequestRepository createCampaignRequestRepository,
            INotificationRepository notificationRepository, IAccountRepository accountRepository, IStatementPhaseRepository statementRepository)
        {
            this.repository = repository;
            _campaignRepository = campaignRepository;
            _memberRepository = memberRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _notificationRepository = notificationRepository;
            _accountRepository = accountRepository;
            _statementRepository = statementRepository;
        }

        public IEnumerable<ProcessingPhase> GetAllProcessingPhases()
        {
            return repository.GetAll();
        }

        public ProcessingPhase? GetById(Guid id)
        {
            return repository.GetById(id);
        }


        public IEnumerable<ProcessingPhase?> GetProcessingPhaseByOMId(Guid omId)
        {
            var om = _organizationManagerRepository.GetById(omId);
            if (om == null) { throw new NotFoundException("Organizaiton manager not found!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByOM != null && r.CreateByOM.Equals(omId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhase>();
            foreach (var item in campaign)
            {
                var processingPhase = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhase != null && processingPhase.IsProcessing == true)
                    listProcessingPhase.Add(processingPhase);
            }
            return listProcessingPhase;
        }

        public IEnumerable<ProcessingPhase?> GetProcessingPhaseByMemberId(Guid memberId)
        {
            var member = _memberRepository.GetById(memberId);
            if (member == null) { throw new NotFoundException("Member not found!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByMember != null && r.CreateByMember.Equals(memberId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhase>();
            foreach (var item in campaign)
            {
                var processingPhase = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhase != null && processingPhase.IsProcessing == true)
                    listProcessingPhase.Add(processingPhase);
            }
            return listProcessingPhase;
        }


        public IEnumerable<ProcessingPhaseResponse?> GetProcessingPhaseResponseByOMId(Guid omId)
        {
            var om = _organizationManagerRepository.GetById(omId);
            if (om == null) { throw new NotFoundException("Organizaiton manager not found!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByOM.Equals(omId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhaseResponse>();
            foreach (var item in campaign)
            {
                var processingPhase = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhase != null && processingPhase.IsProcessing == true)
                    listProcessingPhase.Add(new ProcessingPhaseResponse
                    {
                        CampaignName = item.Name,
                        ProcessingPhaseId = processingPhase.ProcessingPhaseId
                    });
            }
            return listProcessingPhase;
        }



        public IEnumerable<ProcessingPhaseResponse?> GetProcessingPhaseResponseByMemberId(Guid memberId)
        {
            var member = _memberRepository.GetById(memberId);
            if (member == null) { throw new NotFoundException("Member not found!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByMember.Equals(memberId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhaseResponse>();
            foreach (var item in campaign)
            {
                var processingPhase = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhase != null && processingPhase.IsProcessing == true)
                    listProcessingPhase.Add(new ProcessingPhaseResponse
                    {
                        CampaignName = item.Name,
                        ProcessingPhaseId = processingPhase.ProcessingPhaseId
                    });
            }
            return listProcessingPhase;
        }


        public ProcessingPhase? CreateProcessingPhaseRequest(CreateProcessingPhaseRequest request)
        {
            var campaign = _campaignRepository.GetById(request.CampaignId);
            if (campaign == null) { return null!; }

            var processingPhase = new ProcessingPhase
            {
                ProcessingPhaseId = Guid.NewGuid(),
                CampaignId = request.CampaignId,
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsEnd = request.IsEnd,
                IsProcessing = request.IsProcessing,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
            };

            return repository.Save(processingPhase);
        }


        public void UpdateProcessingPhaseStatus(UpdateProcessingPhaseStatusRequest request)
        {
            var processingPhase = repository.GetById(request.ProcessingPhaseId);
            if (processingPhase == null) { throw new NotFoundException("Processing phase not found!"); }
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) { throw new NotFoundException("Account not found!"); }
            var campaign = _campaignRepository.GetById(processingPhase.CampaignId)!;
            var createCampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;
            if (request.IsEnd == true)
            {
                processingPhase.IsEnd = true;
                processingPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                processingPhase.IsProcessing = false;
                processingPhase.IsLocked = true;
                processingPhase.UpdateBy = request.AccountId;


                var statementPhase = new StatementPhase();
                if (campaign.StatementPhase != null)
                {
                    statementPhase = campaign.StatementPhase;
                    statementPhase.IsProcessing = true;
                    statementPhase.IsLocked = false;
                    statementPhase.IsEnd = false;
                }



                _statementRepository.Update(statementPhase);
                repository.Update(processingPhase);
                if (createCampaignRequest.CreateByOM != null)
                {
                    var om = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = om!.AccountID,
                        Content = $"Trạng thái giai đoạn giải ngân của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
                else if (createCampaignRequest.CreateByMember != null)
                {
                    var member = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByMember);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = member!.AccountID,
                        Content = $"Trạng thái giai đoạn giải ngân của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
            }
            else
            {
                processingPhase.IsEnd = false;
                processingPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                processingPhase.IsProcessing = true;
                processingPhase.UpdateBy = request.AccountId;

                var statementPhase = new StatementPhase();
                if (campaign.StatementPhase != null)
                {
                    statementPhase = campaign.StatementPhase;
                    statementPhase.IsProcessing = false;
                    statementPhase.IsLocked = false;
                    statementPhase.IsEnd = false;
                }



                _statementRepository.Update(statementPhase);
                repository.Update(processingPhase);
                if (createCampaignRequest.CreateByOM != null)
                {
                    var om = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = om!.AccountID,
                        Content = $"Trạng thái giai đoạn giải ngân của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
                else if (createCampaignRequest.CreateByMember != null)
                {
                    var member = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByMember);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = member!.AccountID,
                        Content = $"Trạng thái giai đoạn giải ngân của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
            }
        }


        public void Update(UpdateProcessingPhaseRequest request)
        {
            var processingPhase = repository.GetById(request.ProcessingPhaseId);
            if (processingPhase == null) { throw new NotFoundException("Processing phase not found!"); }

            if (processingPhase.IsLocked) throw new BadRequestException("This phase was locked!");

            if (!String.IsNullOrEmpty(request.Name))
            {
                processingPhase.Name = request.Name;
            }
            if (!String.IsNullOrEmpty(request.StartDate.ToString()))
            {
                processingPhase.StartDate = request.StartDate;
            }
            if (!String.IsNullOrEmpty(request.EndDate.ToString()))
            {
                processingPhase.EndDate = request.EndDate;
            }
            repository.Update(processingPhase);
        }
    }
}
