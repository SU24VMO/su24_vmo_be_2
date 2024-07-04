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
            if(organizationManagerCreated != null)
            {
                _notificationRepository.Save(notification);
            }
            return organizationManagerCreated;
        }


        private void TryValidateRegisterRequest(CreateNewOrganizationManagerRequest request)
        {
            if (new Regex(RegexCollector.PhoneRegex).IsMatch(request.PhoneNumber) == false)
            {
                throw new Exception("Phone is not a valid phone");
            }
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new Exception("Email is not valid.");
            }

            if (string.IsNullOrEmpty(request.FirstName))
            {
                throw new Exception("Firstname must not be null or empty");
            }
            if (string.IsNullOrEmpty(request.LastName))
            {
                throw new Exception("Lastname must not be null or empty");
            }
            if (string.IsNullOrEmpty(request.Gender))
            {
                throw new Exception("Gender must not be null or empty");
            }
            if (_accountRepository.GetByUsername(request.Username) != null)
            {
                throw new Exception("Username was existed!");
            }
            if (_organizationManagerRepository.GetByPhone(request.PhoneNumber) != null)
            {
                throw new Exception("Phone number was existed!");
            }
        }
    }
}
