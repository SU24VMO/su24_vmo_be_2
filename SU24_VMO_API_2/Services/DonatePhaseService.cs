using BusinessObject.Models;
using Org.BouncyCastle.Asn1.Cms;
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
        private readonly IMemberRepository _memberRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly IProcessingPhaseRepository _processingPhaseRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAccountRepository _accountRepository;

        public DonatePhaseService(IDonatePhaseRepository repository, ICampaignRepository campaignRepository, IMemberRepository memberRepository,
            IOrganizationManagerRepository organizationManagerRepository, ICreateCampaignRequestRepository createCampaignRequestRepository,
            INotificationRepository notificationRepository, IAccountRepository accountRepository, IProcessingPhaseRepository processingPhaseRepository)
        {
            _repository = repository;
            _campaignRepository = campaignRepository;
            _memberRepository = memberRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _notificationRepository = notificationRepository;
            _accountRepository = accountRepository;
            _processingPhaseRepository = processingPhaseRepository;
        }


        public IEnumerable<DonatePhase> GetAllDonatePhases()
        {
            return _repository.GetAll();
        }
        public IEnumerable<DonatePhase> GetAllDonatePhasesWithCampaignName(string? campaignName)
        {
            if (!String.IsNullOrEmpty(campaignName))
                return _repository.GetAll().Where(d => d.Campaign.Name.ToLower().Contains(campaignName.ToLower().Trim()));
            else return _repository.GetAll();
        }

        public DonatePhase? GetDonatePhaseById(Guid donatePhaseId)
        {
            return _repository.GetById(donatePhaseId);
        }

        public IEnumerable<DonatePhase?> GetDonatePhaseByOMId(Guid omId)
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

            var listDonatePhase = new List<DonatePhase>();
            foreach (var item in campaign)
            {
                var donatePhase = _repository.GetDonatePhaseByCampaignId(item.CampaignID);
                if (donatePhase != null)
                    listDonatePhase.Add(donatePhase);
            }
            return listDonatePhase;
        }

        public IEnumerable<DonatePhase?> GetDonatePhaseByMemberId(Guid memberId)
        {
            var member = _memberRepository.GetById(memberId);
            if (member == null) { throw new NotFoundException("Thành viên không tìm thấy!"); }
            var listsRequest = _createCampaignRequestRepository.GetAll().Where(r => r.CreateByMember.Equals(memberId));

            var campaign = new List<Campaign>();
            foreach (var item in listsRequest)
            {
                if (item.Campaign != null)
                    campaign.Add(item.Campaign);
            }

            var listDonatePhase = new List<DonatePhase>();
            foreach (var item in campaign)
            {
                var donatePhase = _repository.GetDonatePhaseByCampaignId(item.CampaignID);
                if (donatePhase != null)
                    listDonatePhase.Add(donatePhase);
            }
            return listDonatePhase;
        }



        public DonatePhase? GetPercentDonatePhaseOfCampaignByCampaignId(Guid campaignId)
        {
            var campaign = _campaignRepository.GetById(campaignId);
            if (campaign == null) { throw new NotFoundException("Chiến dịch không tìm thấy!"); }
            var donatePhase = campaign.DonatePhase;
            return donatePhase;
        }

        public void UpdateDonatePhaseStatus(UpdateDonatePhaseStatusRequest request)
        {
            var donatePhase = _repository.GetById(request.DonatePhaseId);
            if (donatePhase == null) { throw new NotFoundException("Giai đoạn quyên góp không tìm thấy!"); }
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) { throw new NotFoundException("Tài khoản không tìm thấy!"); }

            var campaign = _campaignRepository.GetById(donatePhase.CampaignId)!;
            var createCampaignRequest = _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;
            if (request.IsEnd == true)
            {
                donatePhase.IsEnd = true;
                donatePhase.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                donatePhase.IsProcessing = false;
                donatePhase.IsLocked = true;
                donatePhase.UpdateBy = request.AccountId;
                donatePhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);

                var processingPhase = new ProcessingPhase();
                if (campaign.ProcessingPhase != null)
                {
                    processingPhase = campaign.ProcessingPhase;
                    processingPhase.StartDate = TimeHelper.GetTime(DateTime.UtcNow);
                    processingPhase.IsProcessing = true;
                    processingPhase.IsLocked = false;
                    processingPhase.IsEnd = false;
                }



                _processingPhaseRepository.Update(processingPhase);
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
                else if (createCampaignRequest.CreateByMember != null)
                {
                    var member = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember);
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


                var processingPhase = new ProcessingPhase();
                if (campaign.ProcessingPhase != null)
                {
                    processingPhase = campaign.ProcessingPhase;
                    processingPhase.IsProcessing = false;
                    processingPhase.IsLocked = false;
                    processingPhase.IsEnd = false;
                }



                _processingPhaseRepository.Update(processingPhase);
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
                else if (createCampaignRequest.CreateByMember != null)
                {
                    var member = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember);
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

                if (currentValue >= double.Parse(campaign.TargetAmount))
                {
                    campaign.CanBeDonated = false;
                    donatePhase.IsEnd = true;
                    donatePhase.IsProcessing = false;
                    donatePhase.IsLocked = true;
                    donatePhase.EndDate = TimeHelper.GetTime(DateTime.UtcNow);

                    var processingPhase = _processingPhaseRepository.GetProcessingPhaseByCampaignId(campaignId);
                    if (processingPhase != null)
                    {
                        processingPhase.IsProcessing = true;
                        processingPhase.StartDate = TimeHelper.GetTime(DateTime.UtcNow);
                        processingPhase.IsLocked = false;
                        _processingPhaseRepository.Update(processingPhase);
                    }
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
            if (donatePhase == null) { throw new NotFoundException("Giai đoạn quyên góp không tìm thấy!"); }

            if (donatePhase.IsLocked) throw new BadRequestException("Giai đoạn này đã bị khóa!");

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
