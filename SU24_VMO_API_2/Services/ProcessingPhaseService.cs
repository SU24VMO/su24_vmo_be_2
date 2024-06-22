using BusinessObject.Models;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class ProcessingPhaseService
    {
        private readonly IProcessingPhaseRepository repository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly INotificationRepository _notificationRepository;

        public ProcessingPhaseService(IProcessingPhaseRepository repository, ICampaignRepository campaignRepository, IUserRepository userRepository, 
            IOrganizationManagerRepository organizationManagerRepository, ICreateCampaignRequestRepository createCampaignRequestRepository, 
            INotificationRepository notificationRepository)
        {
            this.repository = repository;
            _campaignRepository = campaignRepository;
            _userRepository = userRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _notificationRepository = notificationRepository;
        }

        public IEnumerable<ProcessingPhase> GetAllProcessingPhases()
        {
            return repository.GetAll();
        }

        public ProcessingPhase? GetById(Guid id)
        {
            return repository.GetById(id);
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
            var campaign = _campaignRepository.GetById(processingPhase.CampaignId)!;
            var createCampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;
            if (request.IsEnd == true)
            {
                processingPhase.IsEnd = true;
                processingPhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                processingPhase.IsProcessing = false;
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
                else if (createCampaignRequest.CreateByUser != null)
                {
                    var member = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByUser);
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
                else if (createCampaignRequest.CreateByUser != null)
                {
                    var member = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByUser);
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
            if (processingPhase == null) { return; }
            if(!String.IsNullOrEmpty(request.Name))
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
            if (request.IsEnd != null)
            {
                processingPhase.IsEnd = (bool)request.IsEnd;
            }
            if (request.IsProcessing != null)
            {
                processingPhase.IsProcessing = (bool)request.IsProcessing;
            }
            repository.Update(processingPhase);
        }
    }
}
