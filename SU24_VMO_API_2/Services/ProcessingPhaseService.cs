using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Response;
using System.Text;
using BusinessObject.Enums;
using System.Diagnostics;

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
        private readonly IDonatePhaseRepository _donatePhaseRepository;
        private readonly ITransactionRepository _transactionRepository;

        public ProcessingPhaseService(IProcessingPhaseRepository repository, ICampaignRepository campaignRepository, IMemberRepository memberRepository,
            IOrganizationManagerRepository organizationManagerRepository, ICreateCampaignRequestRepository createCampaignRequestRepository,
            INotificationRepository notificationRepository, IAccountRepository accountRepository, IStatementPhaseRepository statementRepository,
            IDonatePhaseRepository donatePhaseRepository, ITransactionRepository transactionRepository)
        {
            this.repository = repository;
            _campaignRepository = campaignRepository;
            _memberRepository = memberRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _notificationRepository = notificationRepository;
            _accountRepository = accountRepository;
            _statementRepository = statementRepository;
            _donatePhaseRepository = donatePhaseRepository;
            _transactionRepository = transactionRepository;
        }

        public IEnumerable<ProcessingPhase> GetAllProcessingPhases()
        {
            return repository.GetAll();
        }

        public IEnumerable<ProcessingPhase> GetAllProcessingPhasesWithCampaignName(string? campaignName)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                string normalizedCampaignName = campaignName.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
                return repository.GetAll().Where(a => a.Campaign.Name.ToLowerInvariant().Normalize(NormalizationForm.FormD).Contains(normalizedCampaignName));
            }

            else return repository.GetAll();
        }

        public ProcessingPhase? GetById(Guid id)
        {
            return repository.GetById(id);
        }


        public IEnumerable<ProcessingPhase>? GetProcessingPhaseByOMId(Guid omId)
        {
            var om = _organizationManagerRepository.GetById(omId);
            if (om == null) { throw new NotFoundException("Quản lý tổ chức không tồn tại!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByOM != null && r.CreateByOM.Equals(omId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null && item.Campaign.CampaignTier == CampaignTier.FullDisbursementCampaign && item.Campaign.IsActive)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhase>();
            foreach (var item in campaign)
            {
                var processingPhases = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhases != null && processingPhases.Any(pp => pp.IsProcessing) == true)
                    foreach (var processingPhase in processingPhases)
                    {
                        if (processingPhase.IsProcessing)
                        {
                            listProcessingPhase.Add(processingPhase);
                        }
                    }
            }
            return listProcessingPhase;
        }

        public IEnumerable<ProcessingPhase>? GetProcessingPhaseActiveTierIIByOMId(Guid omId)
        {
            var om = _organizationManagerRepository.GetById(omId);
            if (om == null) { throw new NotFoundException("Quản lý tổ chức không tồn tại!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByOM != null && r.CreateByOM.Equals(omId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null && item.Campaign.CampaignTier == CampaignTier.PartialDisbursementCampaign && item.Campaign.IsActive)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhase>();
            foreach (var item in campaign)
            {
                var processingPhases = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhases != null && processingPhases.Any(pp => pp.IsProcessing) == true)
                    foreach (var processingPhase in processingPhases)
                    {
                        if (processingPhase.IsProcessing)
                        {
                            listProcessingPhase.Add(processingPhase);
                        }
                    }
            }
            return listProcessingPhase;
        }


        public IEnumerable<ProcessingPhase>? GetProcessingPhaseUnActiveTierIIByOMId(Guid omId)
        {
            var om = _organizationManagerRepository.GetById(omId);
            if (om == null) { throw new NotFoundException("Quản lý tổ chức không tồn tại!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByOM != null && r.CreateByOM.Equals(omId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null && item.Campaign.CampaignTier == CampaignTier.PartialDisbursementCampaign && !item.Campaign.IsActive)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhase>();
            foreach (var item in campaign)
            {
                var processingPhases = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhases != null && processingPhases.Any(pp => pp.IsProcessing) == true)
                    foreach (var processingPhase in processingPhases)
                    {
                        listProcessingPhase.Add(processingPhase);
                    }
            }
            return listProcessingPhase;
        }


        public IEnumerable<ProcessingPhase>? GetProcessingPhaseByMemberId(Guid memberId)
        {
            var member = _memberRepository.GetById(memberId);
            if (member == null) { throw new NotFoundException("Thành viên không tồn tại!"); }
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
                var processingPhases = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhases != null && processingPhases.Any(pp => pp.IsProcessing) == true)
                    foreach (var processingPhase in processingPhases)
                    {
                        if (processingPhase.IsProcessing)
                        {
                            listProcessingPhase.Add(processingPhase);
                        }
                    }
            }
            return listProcessingPhase;
        }

        public IEnumerable<ProcessingPhase>? GetProcessingPhaseActiveTierIIByMemberId(Guid memberId)
        {
            var member = _memberRepository.GetById(memberId);
            if (member == null) { throw new NotFoundException("Thành viên không tồn tại!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByMember != null && r.CreateByMember.Equals(memberId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null && item.Campaign.CampaignTier == CampaignTier.PartialDisbursementCampaign && item.Campaign.IsActive)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhase>();
            foreach (var item in campaign)
            {
                var processingPhases = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhases != null && processingPhases.Any(pp => pp.IsProcessing) == true)
                    foreach (var processingPhase in processingPhases)
                    {
                        if (processingPhase.IsProcessing)
                        {
                            listProcessingPhase.Add(processingPhase);
                        }
                    }
            }
            return listProcessingPhase;
        }

        public IEnumerable<ProcessingPhase>? GetProcessingPhaseUnActiveTierIIByMemberId(Guid memberId)
        {
            var member = _memberRepository.GetById(memberId);
            if (member == null) { throw new NotFoundException("Thành viên không tồn tại!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByMember != null && r.CreateByMember.Equals(memberId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null && item.Campaign.CampaignTier == CampaignTier.PartialDisbursementCampaign && !item.Campaign.IsActive)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhase>();
            foreach (var item in campaign)
            {
                var processingPhases = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhases != null && processingPhases.Any(pp => pp.IsProcessing) == true)
                    foreach (var processingPhase in processingPhases)
                    {
                        listProcessingPhase.Add(processingPhase);
                    }
            }
            return listProcessingPhase;
        }


        public IEnumerable<ProcessingPhaseResponse>? GetProcessingPhaseResponseByOMId(Guid omId)
        {
            var om = _organizationManagerRepository.GetById(omId);
            if (om == null) { throw new NotFoundException("Quản lý tổ chức không tồn tại!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByOM.Equals(omId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null && item.Campaign.CampaignTier == CampaignTier.FullDisbursementCampaign)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhaseResponse>();
            foreach (var item in campaign)
            {
                var processingPhases = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhases != null && processingPhases.Any(pp => pp.IsProcessing) == true)
                    foreach (var processingPhase in processingPhases)
                    {
                        listProcessingPhase.Add(new ProcessingPhaseResponse
                        {
                            CampaignName = item.Name,
                            ProcessingPhaseId = processingPhase.ProcessingPhaseId
                        });
                    }
            }
            return listProcessingPhase;
        }



        public IEnumerable<ProcessingPhaseResponse?> GetProcessingPhaseResponseByMemberId(Guid memberId)
        {
            var member = _memberRepository.GetById(memberId);
            if (member == null) { throw new NotFoundException("Thành viên không tồn tại!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByMember.Equals(memberId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null && item.Campaign.CampaignTier == CampaignTier.FullDisbursementCampaign)
                    campaign.Add(item.Campaign);
            }

            var listProcessingPhase = new List<ProcessingPhaseResponse>();
            foreach (var item in campaign)
            {
                var processingPhases = repository.GetProcessingPhaseByCampaignId(item.CampaignID);
                if (processingPhases != null && processingPhases.Any(pp => pp.IsProcessing) == true)
                    foreach (var processingPhase in processingPhases)
                    {
                        listProcessingPhase.Add(new ProcessingPhaseResponse
                        {
                            CampaignName = item.Name,
                            ProcessingPhaseId = processingPhase.ProcessingPhaseId
                        });
                    }
            }
            return listProcessingPhase;
        }


        public ProcessingPhaseResponseForCampaignTierIIWithTotalItem GetProcessingPhaseResponseForCampaignTierII(Guid accountId, int? pageSize, int? pageNo, string? processingPhaseName)
        {
            var account = _accountRepository.GetById(accountId);
            if (account == null) throw new NotFoundException("Tài khoản này không tồn tại!");
            var processingPhases = repository.GetAllByAccountId(accountId, pageSize, pageNo, processingPhaseName);
            processingPhases.Item1 = processingPhases.Item1.OrderBy(p => p.Priority);
            var response = MapProcessingPhaseToResponse(processingPhases.Item1.ToList());
            foreach (var item in processingPhases.Item1)
            {
                if (item.Campaign != null) item.Campaign.ProcessingPhases = null;
                double targetAmount = double.Parse(item.Campaign.TargetAmount);
                item.Campaign.Transactions = (_transactionRepository.GetTransactionByCampaignId(item.CampaignId) ?? Array.Empty<Transaction>()).ToList();
                double currentPercent = item.Campaign.Transactions.Where(t =>
                    t.TransactionStatus == TransactionStatus.Success &&
                    t.TransactionType == TransactionType.Receive).Sum(t => t.Amount);
                item.CurrentPercent =
                    Math.Round((currentPercent / targetAmount) * 100, 3);
                var listProcessingPhaseBeforeCurrentPriority =
                    processingPhases.Item1.Where(p => p.Priority <= item.Priority);
                var percentBeforePriority =
                    listProcessingPhaseBeforeCurrentPriority.Sum(p => p.Percent);
                foreach (var processing in response)
                {
                    if (item.CurrentPercent >= Math.Floor((double)percentBeforePriority))
                    {
                        if (processing.ProcessingPhaseId.Equals(item.ProcessingPhaseId))
                        {
                            processing.IsEligible = true;
                        }
                    }
                }
            }
            return new ProcessingPhaseResponseForCampaignTierIIWithTotalItem
            {
                ProcessingPhases = response,
                TotalItem = processingPhases.Item2
            };
        }

        private List<ProcessingPhaseResponseForCampaignTierII>? MapProcessingPhaseToResponse(List<ProcessingPhase> phases)
        {
            return phases?.Select(phase => new ProcessingPhaseResponseForCampaignTierII()
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
            }).ToList();
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
            if (processingPhase == null) { throw new NotFoundException("Không tìm thấy giai đoạn giải ngân!"); }
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) { throw new NotFoundException("Tài khoản không tìm thấy!"); }
            var campaign = _campaignRepository.GetById(processingPhase.CampaignId)!;
            var createCampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;
            if (request.IsEnd == true)
            {
                processingPhase.IsEnd = true;
                processingPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                processingPhase.IsProcessing = false;
                processingPhase.IsLocked = true;
                processingPhase.UpdateBy = request.AccountId;
                processingPhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);



                var statementPhase = new StatementPhase();
                if (campaign.StatementPhase != null)
                {
                    statementPhase = campaign.StatementPhase;
                    statementPhase.StartDate = TimeHelper.GetTime(DateTime.UtcNow);
                    statementPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
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
                    var member = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember);
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
                    statementPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
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
                    var member = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember);
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


        public void UpdateProcessingPhaseToNextProcessingPhaseOfCampaignTierII(UpdateProcessingPhaseStatusRequest request)
        {
            var processingPhase = repository.GetById(request.ProcessingPhaseId);
            if (processingPhase == null) { throw new NotFoundException("Không tìm thấy giai đoạn giải ngân!"); }
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) { throw new NotFoundException("Tài khoản không tìm thấy!"); }
            var campaign = _campaignRepository.GetById(processingPhase.CampaignId)!;
            if (campaign.CampaignTier != CampaignTier.PartialDisbursementCampaign)
            {
                throw new BadRequestException(
                    "Chiến dịch bạn mong muốn cập nhật không phải là chiến dịch giải ngân từng phần!");
            }
            var createCampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;
            if (request.IsEnd == true)
            {
                var maxPriority = repository.GetProcessingPhaseByCampaignId(campaign.CampaignID).Max(p => p.Priority);
                var finalProcessingPhase = repository.GetProcessingPhaseByCampaignId(campaign.CampaignID)
                    .FirstOrDefault(p => p.Priority == maxPriority);

                if (processingPhase.Priority != maxPriority)
                {
                    processingPhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);
                    processingPhase.IsProcessing = false;
                    processingPhase.IsActive = true;
                    processingPhase.IsEnd = true;
                    processingPhase.IsLocked = true;
                    repository.Update(processingPhase);

                    var nextProcessingPhase = repository.GetProcessingPhaseByCampaignId(campaign.CampaignID).FirstOrDefault(p => p.Priority == processingPhase.Priority + 1);
                    if (nextProcessingPhase != null)
                    {
                        nextProcessingPhase.StartDate = TimeHelper.GetTime(DateTime.UtcNow);
                        nextProcessingPhase.IsProcessing = true;
                        nextProcessingPhase.IsActive = true;
                        nextProcessingPhase.IsLocked = false;
                        nextProcessingPhase.IsEnd = false;
                        repository.Update(nextProcessingPhase);
                    }
                }
                else
                {
                    processingPhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);
                    processingPhase.IsProcessing = false;
                    processingPhase.IsActive = true;
                    processingPhase.IsEnd = true;
                    processingPhase.IsLocked = true;
                    repository.Update(processingPhase);

                    campaign.IsComplete = true;
                    campaign.ActualEndDate = TimeHelper.GetTime(DateTime.UtcNow);
                    var donatePhase = _donatePhaseRepository.GetDonatePhaseByCampaignId(campaign.CampaignID);
                    if (donatePhase != null)
                    {
                        campaign.CanBeDonated = false;
                        donatePhase.IsEnd = true;
                        donatePhase.IsProcessing = false;
                        donatePhase.IsLocked = true;
                        donatePhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);
                    }
                    _campaignRepository.Update(campaign);
                }

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
                    var member = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember);
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
                    statementPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
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
                    var member = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember);
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
            if (processingPhase == null) { throw new NotFoundException("Không tìm thấy giai đoạn giải ngân!"); }

            if (processingPhase.IsLocked) throw new BadRequestException("Giai đoạn này hiện không thể chỉnh sửa!");

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
