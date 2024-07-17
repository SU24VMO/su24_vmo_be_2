using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Request;

namespace SU24_VMO_API.Services
{
    public class StatementPhaseService
    {
        private readonly IStatementPhaseRepository _repository;
        private readonly IStatementFileRepository _statementFileRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IMemberRepository _userRepository;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAccountRepository _accountRepository;

        public StatementPhaseService(IStatementPhaseRepository repository, ICampaignRepository campaignRepository,
            ICreateCampaignRequestRepository createCampaignRequestRepository, IOrganizationManagerRepository organizationManagerRepository,
            INotificationRepository notificationRepository, IAccountRepository accountRepository, IMemberRepository userRepository,
            IStatementFileRepository statementFileRepository)
        {
            _repository = repository;
            _campaignRepository = campaignRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _notificationRepository = notificationRepository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _statementFileRepository = statementFileRepository;
        }

        public IEnumerable<StatementPhase> GetAll()
        {
            return _repository.GetAll();
        }

        public IEnumerable<StatementPhase> GetAllStatementPhasesWithCampaignName(string? campaignName)
        {
            if (!String.IsNullOrEmpty(campaignName))
                return _repository.GetAll().Where(d => d.Campaign.Name.ToLower().Contains(campaignName.ToLower().Trim()));
            else return _repository.GetAll();
        }

        public StatementPhase? GetById(Guid id)
        {
            return _repository.GetById(id);
        }


        public IEnumerable<StatementPhase?> GetStatementPhaseByOMId(Guid omId)
        {
            var om = _organizationManagerRepository.GetById(omId);
            if (om == null) { throw new NotFoundException("Quản lý tổ chức không tìm thấy!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByOM.Equals(omId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null)
                    campaign.Add(item.Campaign);
            }

            var listStatementPhase = new List<StatementPhase>();
            foreach (var item in campaign)
            {
                var statementPhase = _repository.GetStatementPhaseByCampaignId(item.CampaignID);
                if (statementPhase != null)
                {
                    listStatementPhase.Add(statementPhase);
                    var statementFiles = _statementFileRepository.GetAll().Where(s => s.StatementPhaseId.Equals(statementPhase.StatementPhaseId));
                    foreach (var statementFile in statementFiles)
                    {
                        statementFile.StatementPhase = null;
                    }
                    statementPhase.StatementFiles = statementFiles.ToList();
                }

            }
            return listStatementPhase;
        }

        public IEnumerable<StatementPhase?> GetStatementPhaseByMemberId(Guid userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null) { throw new NotFoundException("Người dùng không tìm thấy!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByMember.Equals(userId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null)
                    campaign.Add(item.Campaign);
            }

            var listStatementPhase = new List<StatementPhase>();
            foreach (var item in campaign)
            {
                var statementPhase = _repository.GetStatementPhaseByCampaignId(item.CampaignID);
                if (statementPhase != null)
                {
                    listStatementPhase.Add(statementPhase);
                    var statementFiles = _statementFileRepository.GetAll().Where(s => s.StatementPhaseId.Equals(statementPhase.StatementPhaseId));
                    foreach (var statementFile in statementFiles)
                    {
                        statementFile.StatementPhase = null;
                    }
                    statementPhase.StatementFiles = statementFiles.ToList();
                }
            }
            return listStatementPhase;
        }


        public StatementPhase? CreateStatementPhase(CreateStatementPhaseRequest request)
        {
            if (_campaignRepository.GetById(request.CampaignId) == null)
            {
                return null;
            }
            var statementPhase = new StatementPhase
            {
                StatementPhaseId = Guid.NewGuid(),
                CampaignId = request.CampaignId,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsEnd = request.IsEnd,
                IsProcessing = request.IsProcessing,
            };

            return _repository.Save(statementPhase);
        }


        public void UpdateStatementPhaseStatus(UpdateStatementPhaseStatusRequest request)
        {
            var statementPhase = _repository.GetById(request.StatementPhaseId);
            if (statementPhase == null) { throw new NotFoundException("Giai đoạn sao kê không tìm thấy!"); }
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) { throw new NotFoundException("Tài khoản không tìm thấy!"); }
            var campaign = _campaignRepository.GetById(statementPhase.CampaignId)!;
            var createCampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;
            if (request.IsEnd == true)
            {
                statementPhase.IsEnd = true;
                statementPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                statementPhase.IsProcessing = false;
                statementPhase.IsLocked = true;
                statementPhase.UpdateBy = request.AccountId;
                statementPhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);
                _repository.Update(statementPhase);
                campaign.IsComplete = true;
                campaign.ActualEndDate = TimeHelper.GetTime(DateTime.UtcNow);
                _campaignRepository.Update(campaign);
                if (createCampaignRequest.CreateByOM != null)
                {
                    var om = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = om!.AccountID,
                        Content = $"Trạng thái giai đoạn sao kê của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
                else if (createCampaignRequest.CreateByMember != null)
                {
                    var member = _userRepository.GetById((Guid)createCampaignRequest.CreateByMember);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = member!.AccountID,
                        Content = $"Trạng thái giai đoạn sao kê của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
            }
            else
            {
                statementPhase.IsEnd = false;
                statementPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                statementPhase.IsProcessing = true;
                statementPhase.UpdateBy = request.AccountId;
                _repository.Update(statementPhase);
                campaign.IsComplete = false;
                campaign.ActualEndDate = TimeHelper.GetTime(DateTime.UtcNow);
                _campaignRepository.Update(campaign);
                if (createCampaignRequest.CreateByOM != null)
                {
                    var om = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = om!.AccountID,
                        Content = $"Trạng thái giai đoạn sao kê của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
                else if (createCampaignRequest.CreateByMember != null)
                {
                    var member = _userRepository.GetById((Guid)createCampaignRequest.CreateByMember);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = member!.AccountID,
                        Content = $"Trạng thái giai đoạn sao kê của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
            }
        }


        public void UpdateStatementPhase(UpdateStatementPhaseRequest request)
        {
            var statementPhase = _repository.GetById(request.StatementPhaseId);
            if (statementPhase == null) { throw new NotFoundException("Giai đoạn sao kê không tìm thấy!"); }

            if (statementPhase.IsLocked) throw new BadRequestException("Giai đoạn này hiện không thể chỉnh sửa!");

            if (!String.IsNullOrEmpty(request.Name))
            {
                statementPhase.Name = request.Name;
            }
            if (!String.IsNullOrEmpty(request.StartDate.ToString()))
            {
                statementPhase.StartDate = request.StartDate;
            }
            if (!String.IsNullOrEmpty(request.EndDate.ToString()))
            {
                statementPhase.EndDate = request.EndDate;
            }
            _repository.Update(statementPhase);
        }
    }
}
