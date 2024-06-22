using BusinessObject.Enums;
using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class CreateOrganizationManagerRequestService
    {
        private readonly ICreateOrganizationManagerRequestRepository _createOrganizationManagerRequestRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationRepository _notificationRepository;



        public CreateOrganizationManagerRequestService(ICreateOrganizationManagerRequestRepository createOrganizationManagerRequestRepository,
            IAccountRepository accountRepository, INotificationRepository notificationRepository, IOrganizationManagerRepository organizationManagerRepository)
        {
            _createOrganizationManagerRequestRepository = createOrganizationManagerRequestRepository;
            _accountRepository = accountRepository;
            _notificationRepository = notificationRepository;
            _organizationManagerRepository = organizationManagerRepository;
        }

        public IEnumerable<CreateOrganizationManagerRequest>? GetAllCreateOrganizationManagerRequests()
        {
            return _createOrganizationManagerRequestRepository.GetAll();
        }

        public CreateOrganizationManagerRequest? CreateOrganizationManagerVerifiedRequest(CreateOrganizationManagerVerifiedRequest request)
        {
            //    TryValidateRegisterRequest(request);
            if (_organizationManagerRepository.GetById(request.OrganizationManagerID) == null) return null;
            //    //PasswordUtils.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var createOrganizationVerifiedRequest = new CreateOrganizationManagerRequest
            {
                CreateOrganizationManagerRequestID = Guid.NewGuid(),
                OrganizationManagerID = request.OrganizationManagerID,
                Name = request.Name,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                PersonalTaxCode = request.PersonalTaxCode,
                CitizenIdentification = request.CitizenIdentification,
                CreateBy = request.OrganizationManagerID,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsAcceptTermOfUse = request.IsAcceptTermOfUse,
                IsApproved = false,
                IsPending = true,
                IsRejected = false,
                IsLocked = false,
            };

            var organizationManager = _organizationManagerRepository.GetById(request.OrganizationManagerID);
            var account = _accountRepository.GetById(organizationManager!.AccountID);


            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account!.AccountID,
                Content = "Yêu cầu trở thành tài khoản quản lý được xác minh của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };


            var createOrganizationVerifiedRequestCreated = _createOrganizationManagerRequestRepository.Save(createOrganizationVerifiedRequest);
            if (createOrganizationVerifiedRequestCreated != null)
            {
                _notificationRepository.Save(notification);
            }

            return createOrganizationVerifiedRequestCreated;
        }


        public bool AcceptOrRejectCreateOrganizationManagerAccountRequest(UpdateCreateOrganizationManagerVerifiedAccountRequest updateCreateOrganizationManagerVerifiedAccount)
        {
            TryValidateUpdateCreateOrganizationManagerRequest(updateCreateOrganizationManagerVerifiedAccount);
            var request = _createOrganizationManagerRequestRepository.GetById((Guid)updateCreateOrganizationManagerVerifiedAccount.CreateOrganizationManagerRequestID!);
            if (request == null) { throw new NotFoundException("Request not found!"); }
            var organizationManager = _organizationManagerRepository.GetById(request.CreateBy);
            var account = _accountRepository.GetById(organizationManager!.AccountID);
            var result = false;
            if (request == null)
            {
                return result;
            }


            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account!.AccountID,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };

            if (updateCreateOrganizationManagerVerifiedAccount.IsApproved == true)
            {
                organizationManager = _organizationManagerRepository.GetById(request.OrganizationManagerID);
                organizationManager!.IsVerified = true;

                request.IsApproved = true;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = false;
                result = true;
                notification.Content = "Yêu cầu trở thành tài khoản quản lý được xác minh của bạn vừa được duyệt thành công, hãy trải nghiệm dịch vụ nhé!";
            }
            else
            {
                organizationManager = _organizationManagerRepository.GetById(request.OrganizationManagerID);
                organizationManager!.IsVerified = false;

                request.IsApproved = false;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = true;
                result = true;
                notification.Content = "Yêu cầu trở thành tài khoản quản lý được xác minh của bạn chưa được công nhận, hãy cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
            }



            _createOrganizationManagerRequestRepository.Update(request);
            _organizationManagerRepository.Update(organizationManager);
            _accountRepository.Update(account);
            _notificationRepository.Save(notification);
            return result;
        }

        private void TryValidateUpdateCreateOrganizationManagerRequest(UpdateCreateOrganizationManagerVerifiedAccountRequest request)
        {
            if (String.IsNullOrEmpty(request.CreateOrganizationManagerRequestID.ToString()))
            {
                throw new Exception("Id must not be null or empty!");
            }
            if (String.IsNullOrEmpty(request.IsApproved.ToString()))
            {
                throw new Exception("Status must not be null or empty!");
            }
        }
    }
}
