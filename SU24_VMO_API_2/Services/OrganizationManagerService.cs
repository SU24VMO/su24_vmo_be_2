using BusinessObject.Enums;
using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.JWTAuthSupport;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API.Supporters.Utils;
using System.Text.RegularExpressions;

namespace SU24_VMO_API.Services
{
    public class OrganizationManagerService
    {
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly JwtTokenSupporter jwtTokenSupporter;
        private readonly INotificationRepository _notificationRepository;
        private readonly IDBTransactionRepository _transactionRepository;


        public OrganizationManagerService(IOrganizationManagerRepository organizationManagerRepository, IAccountRepository accountRepository,
            JwtTokenSupporter jwtTokenSupporter, INotificationRepository notificationRepository, IDBTransactionRepository transactionRepository)
        {
            _organizationManagerRepository = organizationManagerRepository;
            _accountRepository = accountRepository;
            this.jwtTokenSupporter = jwtTokenSupporter;
            _notificationRepository = notificationRepository;
            _transactionRepository = transactionRepository;
        }

        public IEnumerable<OrganizationManager>? GetAllOrganizationManagers()
        {
            return _organizationManagerRepository.GetAll();
        }

        public IEnumerable<OrganizationManager>? GetAllOrganizationManagersByOrganizationManagerName(string organizationManagerName)
        {
            var organizationManagers = _organizationManagerRepository.GetAll().Where(m => (m.FirstName.Trim().ToLower() + " " + m.LastName.Trim().ToLower()).Contains(organizationManagerName.ToLower().Trim()));
            foreach (var organizationManager in organizationManagers)
            {
                if (organizationManager.Organizations != null)
                    organizationManager.Organizations.Clear();
                if (organizationManager.CreateCampaignRequests != null)
                    organizationManager.CreateCampaignRequests.Clear();
                if (organizationManager.CreateActivityRequests != null)
                    organizationManager.CreateActivityRequests.Clear();
                if (organizationManager.CreateOrganizationRequests != null)
                    organizationManager.CreateOrganizationRequests.Clear();
                if (organizationManager.CreatePostRequests != null)
                    organizationManager.CreatePostRequests.Clear();
            }
            return organizationManagers;
        }


        public OrganizationManager? CreateOrganizationManager(CreateNewOrganizationManagerRequest request)
        {

            TryValidateRegisterRequest(request);
            if (_accountRepository.GetByEmail(request.Email) != null) return null;
            PasswordUtils.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var account = new Account
            {
                AccountID = Guid.NewGuid(),
                HashPassword = passwordHash,
                SaltPassword = passwordSalt,
                Email = request.Email,
                Username = request.Username,
                Avatar = request.Avatar,
                Role = Role.OrganizationManager,
                IsActived = true,
                IsBlocked = false,
                CreatedAt = TimeHelper.GetTime(DateTime.UtcNow),
                UpdatedAt = null,
            };

            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account.AccountID,
                Content = "Tài khoản của bạn vừa được tạo thành công, hãy trải nghiệm và chia sẻ ứng dụng của chúng tôi đến mọi người nhé!",
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };

            var organizationManager = new OrganizationManager()
            {
                OrganizationManagerID = Guid.NewGuid(),
                AccountID = account.AccountID,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                BirthDay = request.BirthDay,
                FacebookUrl = request.FacebookUrl,
                TiktokUrl = request.TiktokUrl,
                YoutubeUrl = request.YoutubeUrl,
                IsVerified = false,
                Account = account,
            };

            var organizationManagerCreated = _organizationManagerRepository.Save(organizationManager);
            if (organizationManagerCreated != null)
            {
                _notificationRepository.Save(notification);
            }
            return organizationManagerCreated;
        }


        private void TryValidateRegisterRequest(CreateNewOrganizationManagerRequest request)
        {
            if (new Regex(RegexCollector.PhoneRegex).IsMatch(request.PhoneNumber) == false)
            {
                throw new Exception("Số điện thoại không hợp lệ");
            }
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new Exception("Email không hợp lệ.");
            }

            if (string.IsNullOrEmpty(request.FirstName))
            {
                throw new Exception("Họ không được để trống");
            }
            if (string.IsNullOrEmpty(request.LastName))
            {
                throw new Exception("Tên không được để trống");
            }
            if (string.IsNullOrEmpty(request.Gender))
            {
                throw new Exception("Giới tính không được để trống");
            }
            if (_accountRepository.GetByUsername(request.Username) != null)
            {
                throw new Exception("Tên người dùng đã tồn tại!");
            }
            if (_organizationManagerRepository.GetByPhone(request.PhoneNumber) != null)
            {
                throw new Exception("Số điện thoại đã tồn tại!");
            }
        }
    }
}
