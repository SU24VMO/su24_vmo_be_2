

using BusinessObject.Models;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;
using System.Text.RegularExpressions;

namespace SU24_VMO_API.Services
{
    public class NotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IAccountRepository _accountRepository;


        public NotificationService(INotificationRepository notificationRepository, IAccountRepository accountRepository)
        {
            _notificationRepository = notificationRepository;
            _accountRepository = accountRepository;
        }


        public List<Notification> GetAllNotification()
        {
            return _notificationRepository.GetAll().ToList();
        }

        public Notification? CreateNotificationNewAccountRequest(CreateNotificationRequest createNotificationRequest)
        {
            TryValidateCreateNotificationRequest(createNotificationRequest);
            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                AccountID = createNotificationRequest.AccountID,
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                Content = "Tài khoản của bạn vừa được tạo thành công, hãy trải nghiệm và chia sẻ ứng dụng của chúng tôi đến mọi người nhé!",
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };
            var notificationAdded = _notificationRepository.Save(notification);
            return notificationAdded;
        }

        private void TryValidateCreateNotificationRequest(CreateNotificationRequest request)
        {
            var account = _accountRepository.GetById(request.AccountID);
            if (account == null)
            {
                throw new Exception("Account not found!.");
            }
        }
    }
}
