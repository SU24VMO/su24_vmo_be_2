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
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAccountRepository _accountRepository;

        public StatementPhaseService(IStatementPhaseRepository repository, ICampaignRepository campaignRepository,
            ICreateCampaignRequestRepository createCampaignRequestRepository, IOrganizationManagerRepository organizationManagerRepository,
            INotificationRepository notificationRepository, IAccountRepository accountRepository, IUserRepository userRepository)
        {
            _repository = repository;
            _campaignRepository = campaignRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _notificationRepository = notificationRepository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
        }

        public IEnumerable<StatementPhase> GetAll()
        {
            return _repository.GetAll();
        }

        public StatementPhase? GetById(Guid id)
        {
            return _repository.GetById(id);
        }


        public IEnumerable<StatementPhase?> GetStatementPhaseByOMId(Guid omId)
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

            var listStatementPhase = new List<StatementPhase>();
            foreach (var item in campaign)
            {
                var statementPhase = _repository.GetStatementPhaseByCampaignId(item.CampaignID);
                if (statementPhase != null)
                    listStatementPhase.Add(statementPhase);
            }
            return listStatementPhase;
        }

        public IEnumerable<StatementPhase?> GetStatementPhaseByUserId(Guid userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null) { throw new NotFoundException("User not found!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByUser.Equals(userId));

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
                    listStatementPhase.Add(statementPhase);
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
            if (statementPhase == null) { throw new NotFoundException("Statement phase not found!"); }
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) { throw new NotFoundException("Account not found!"); }
            var campaign = _campaignRepository.GetById(statementPhase.CampaignId)!;
            var createCampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;
            if (request.IsEnd == true)
            {
                statementPhase.IsEnd = true;
                statementPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                statementPhase.IsProcessing = false;
                statementPhase.IsLocked = true;
                statementPhase.UpdateBy = request.AccountId;
                _repository.Update(statementPhase);
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
                else if (createCampaignRequest.CreateByUser != null)
                {
                    var member = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByUser);
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
                else if (createCampaignRequest.CreateByUser != null)
                {
                    var member = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByUser);
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
            if (statementPhase == null) { throw new NotFoundException("Statement phase not found!"); }

            if (statementPhase.IsLocked) throw new BadRequestException("This phase was locked!");

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
