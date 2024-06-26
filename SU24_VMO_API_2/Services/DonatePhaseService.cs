using BusinessObject.Models;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Request;

namespace SU24_VMO_API.Services
{
    public class DonatePhaseService
    {
        private readonly IDonatePhaseRepository _repository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAccountRepository _accountRepository;

        public DonatePhaseService(IDonatePhaseRepository repository, ICampaignRepository campaignRepository, IUserRepository userRepository,
            IOrganizationManagerRepository organizationManagerRepository, ICreateCampaignRequestRepository createCampaignRequestRepository,
            INotificationRepository notificationRepository, IAccountRepository accountRepository)
        {
            _repository = repository;
            _campaignRepository = campaignRepository;
            _userRepository = userRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _notificationRepository = notificationRepository;
            _accountRepository = accountRepository;
        }


        public IEnumerable<DonatePhase> GetAllDonatePhases()
        {
            return _repository.GetAll();
        }

        public void UpdateDonatePhaseStatus(UpdateDonatePhaseStatusRequest request)
        {
            var donatePhase = _repository.GetById(request.DonatePhaseId);
            if (donatePhase == null) { throw new NotFoundException("Donate phase not found!"); }
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) { throw new NotFoundException("Account not found!"); }

            var campaign = _campaignRepository.GetById(donatePhase.CampaignId)!;
            var createCampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;
            if (request.IsEnd == true)
            {
                donatePhase.IsEnd = true;
                donatePhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                donatePhase.IsProcessing = false;
                donatePhase.IsLocked = true;
                donatePhase.UpdateBy = request.AccountId;
                _repository.Update(donatePhase);
                if (createCampaignRequest.CreateByOM != null)
                {
                    var om = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = om!.AccountID,
                        Content = $"Trạng thái giai đoạn ủng hộ của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
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
                        Content = $"Trạng thái giai đoạn ủng hộ của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
            }
            else
            {
                donatePhase.IsEnd = false;
                donatePhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                donatePhase.IsProcessing = true;
                donatePhase.UpdateBy = request.AccountId;
                _repository.Update(donatePhase);
                if (createCampaignRequest.CreateByOM != null)
                {
                    var om = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM);
                    var notificationCreated = _notificationRepository.Save(new Notification
                    {
                        NotificationID = Guid.NewGuid(),
                        NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                        AccountID = om!.AccountID,
                        Content = $"Trạng thái giai đoạn ủng hộ của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
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
                        Content = $"Trạng thái giai đoạn ủng hộ của chiến dịch {campaign.Name} vừa được cập nhật! Vui lòng kiểm tra thông tin của chiến dịch!",
                        CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                        IsSeen = false,
                    });
                }
            }
        }

        public DonatePhase? CreateDonatePhase(DonatePhase donatePhase)
        {
            return _repository.Save(donatePhase);
        }


        public void UpdateDonatePhaseByCampaignIdAndAmountDonate(Guid campaignId, float amountDonate)
        {
            var donatePhase = _repository.GetDonatePhaseByCampaignId(campaignId);
            if (donatePhase != null)
            {
                var campaign = _campaignRepository.GetById(campaignId)!;

                double currentValue = double.Parse(donatePhase!.CurrentMoney);
                currentValue = currentValue + amountDonate;

                // Calculate percent and round it to 3 decimal places
                double targetAmount = double.Parse(campaign.TargetAmount);
                double percent = Math.Round((currentValue / targetAmount) * 100, 3);

                donatePhase.CurrentMoney = currentValue.ToString();
                donatePhase.Percent = percent;

                if (currentValue == double.Parse(campaign.TargetAmount))
                {
                    campaign.CanBeDonated = false;
                    donatePhase.IsEnd = true;
                    donatePhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);
                    _campaignRepository.Update(campaign);
                }
                _repository.Update(donatePhase);
            }
        }

        public DonatePhase? GetDonatePhaseByCampaignId(Guid campaignId)
        {
            return _repository.GetDonatePhaseByCampaignId(campaignId);
        }

        public void Update(UpdateDonatePhaseRequest request)
        {
            var donatePhase = _repository.GetById(request.DonatePhaseId);
            if (donatePhase == null) { throw new NotFoundException("Donate phase not found!"); }

            if (donatePhase.IsLocked) throw new BadRequestException("This phase was locked!");

            if (!String.IsNullOrEmpty(request.Name))
            {
                donatePhase.Name = request.Name;
            }
            if (!String.IsNullOrEmpty(request.StartDate.ToString()))
            {
                donatePhase.StartDate = request.StartDate;
            }
            if (!String.IsNullOrEmpty(request.EndDate.ToString()))
            {
                donatePhase.EndDate = request.EndDate;
            }
            _repository.Update(donatePhase);
        }

    }
}
