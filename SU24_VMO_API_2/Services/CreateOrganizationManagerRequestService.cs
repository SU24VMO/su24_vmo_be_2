using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.Extensions.Localization;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Request;

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
            var requests = _createOrganizationManagerRequestRepository.GetAll();
            foreach (var request in requests)
            {
                if (request.OrganizationManager != null)
                {
                    request.OrganizationManager.Account = _accountRepository.GetById(request.OrganizationManager.AccountID);
                    if (request.OrganizationManager.Account != null)
                    {
                        request.OrganizationManager.Account.Notifications = null;
                        request.OrganizationManager.Account.AccountTokens = null;
                        request.OrganizationManager.Account.BankingAccounts = null;
                        request.OrganizationManager.Account.Transactions = null;
                    }
                }
            }
            return requests;
        }
        public CreateOrganizationManagerRequest? GetCreateOrganizationManagerRequestById(Guid createOrganizationManagerRequestId)
        {
            return GetAllCreateOrganizationManagerRequests().FirstOrDefault(c =>
                c.CreateOrganizationManagerRequestID.Equals(createOrganizationManagerRequestId));
        }

        public IEnumerable<CreateOrganizationManagerRequest>? GetAllCreateOrganizationManagerRequestsByOrganizationManagerName(string? organizationManagerName)
        {
            if (!String.IsNullOrEmpty(organizationManagerName))
                return _createOrganizationManagerRequestRepository.GetAll().Where(m => (m.OrganizationManager!.FirstName.Trim().ToLower() + " " + m.OrganizationManager.LastName.Trim().ToLower()).Contains(organizationManagerName.ToLower().Trim()));
            else return _createOrganizationManagerRequestRepository.GetAll();
        }

        public CreateOrganizationManagerRequest? CreateOrganizationManagerVerifiedRequest(CreateOrganizationManagerVerifiedRequest request)
        {
            //    TryValidateRegisterRequest(request);
            var om = _organizationManagerRepository.GetById(request.OrganizationManagerID);
            if (om == null) throw new NotFoundException("Không tìm thấy quản lý tổ chức!");
            //    //PasswordUtils.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            //validation cho email
            var createOMRequestExisted = _createOrganizationManagerRequestRepository.GetCreateOrganizationManagerRequestsWithEmail(request.Email);
            if (createOMRequestExisted != null && createOMRequestExisted.Count() > 0)
            {
                var listRequestPending = createOMRequestExisted.Where(c => c.IsPending == true);
                if (listRequestPending.Count() > 0)
                {
                    throw new BadRequestException("Đơn tạo tài khoản quản lý tổ chức xác thực cho tài khoản với email này có vẻ như đã được tạo, vui lòng đợi hệ thống phản hồi!");
                }

                var listRequestApproved = createOMRequestExisted.Where(c => c.IsApproved == true && c.CreateBy.Equals(om.OrganizationManagerID));
                if (listRequestApproved.Count() > 0)
                {
                    throw new BadRequestException("Tài khoản này hiện đã trở thành tài khoản quản lý tổ chức xác thực!");
                }
            }


            if (createOMRequestExisted != null && createOMRequestExisted.Count() > 0 && createOMRequestExisted.Where(c => c.IsApproved == true).Count() > 0)
            {
                throw new BadRequestException("Email này hiện đã được sử dụng! Vui lòng sử dụng email khác!");
            }

            //validation cho phone number
            var createOMRequestExistedWithPhoneNumber = _createOrganizationManagerRequestRepository.GetCreateOrganizationManagerRequestsWithPhoneNumber(request.PhoneNumber);
            if (createOMRequestExistedWithPhoneNumber != null && createOMRequestExistedWithPhoneNumber.Count() > 0)
            {
                var listRequestPending = createOMRequestExistedWithPhoneNumber.Where(c => c.IsPending == true);
                if (listRequestPending.Count() > 0)
                {
                    throw new BadRequestException("Đơn tạo tài khoản quản lý tổ chức xác thực cho tài khoản với số điện thoại này có vẻ như đã được tạo, vui lòng đợi hệ thống phản hồi!");
                }

                var listRequestApproved = createOMRequestExistedWithPhoneNumber.Where(c => c.IsApproved == true && c.CreateBy.Equals(om.OrganizationManagerID));
                if (listRequestApproved.Count() > 0)
                {
                    throw new BadRequestException("Tài khoản này hiện đã trở thành tài khoản quản lý tổ chức xác thực!");
                }
            }


            if (createOMRequestExistedWithPhoneNumber != null && createOMRequestExistedWithPhoneNumber.Count() > 0 && createOMRequestExistedWithPhoneNumber.Where(c => c.IsApproved == true).Count() > 0)
            {
                throw new BadRequestException("Số điện thoại này hiện đã được sử dụng! Vui lòng sử dụng số điện thoại khác!");
            }

            var createOrganizationVerifiedRequest = new CreateOrganizationManagerRequest
            {
                CreateOrganizationManagerRequestID = Guid.NewGuid(),
                OrganizationManagerID = request.OrganizationManagerID,
                Name = request.Name,
                Address = request.Address,
                Email = request.Email,
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
                IsDisable = false,
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


        public void UpdateCreateOrganizationManagerVerifiedRequest(Guid createOrganizationManagerVerifiedRequestId, UpdateCreateOrganizationManagerVerifiedRequest request)
        {
            var requestExisted =
                _createOrganizationManagerRequestRepository.GetById(createOrganizationManagerVerifiedRequestId);
            if (requestExisted == null)
            {
                throw new NotFoundException("Đơn duyệt này không tìm thấy!");
            }

            if (requestExisted.IsApproved)
            {
                throw new BadRequestException("Đơn tạo quản lý tổ chức xác thực này hiện đã được duyệt, vì vậy mọi thông tin về đơn này hiện không thể chỉnh sửa!");
            }

            if (!String.IsNullOrEmpty(request.Address))
            {
                requestExisted.Address = request.Address;
            }

            if (!String.IsNullOrEmpty(request.CitizenIdentification))
            {
                requestExisted.CitizenIdentification = request.CitizenIdentification;
            }

            if (!String.IsNullOrEmpty(request.Email))
            {
                requestExisted.Email = request.Email;
            }
            if (!String.IsNullOrEmpty(request.Name))
            {
                requestExisted.Name = request.Name;
            }
            if (!String.IsNullOrEmpty(request.PersonalTaxCode))
            {
                requestExisted.PersonalTaxCode = request.PersonalTaxCode;
            }
            if (!String.IsNullOrEmpty(request.PhoneNumber))
            {
                requestExisted.PhoneNumber = request.PhoneNumber;
            }

            request.IsAcceptTermOfUse = requestExisted.IsAcceptTermOfUse;
            requestExisted.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);

            _createOrganizationManagerRequestRepository.Update(requestExisted);
        }


        public bool AcceptOrRejectCreateOrganizationManagerAccountRequest(UpdateCreateOrganizationManagerVerifiedAccountRequest updateCreateOrganizationManagerVerifiedAccount)
        {
            TryValidateUpdateCreateOrganizationManagerRequest(updateCreateOrganizationManagerVerifiedAccount);
            var request = _createOrganizationManagerRequestRepository.GetById((Guid)updateCreateOrganizationManagerVerifiedAccount.CreateOrganizationManagerRequestID!);
            if (request == null) { throw new NotFoundException("Không tìm thấy yêu cầu này!"); }
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

                request.ApprovedBy = updateCreateOrganizationManagerVerifiedAccount.ModeratorId;
                request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.IsApproved = true;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = false;
                request.IsDisable = false;
                result = true;
                notification.Content = "Yêu cầu trở thành tài khoản quản lý được xác minh của bạn vừa được duyệt thành công, hãy trải nghiệm dịch vụ nhé!";
            }
            else
            {
                organizationManager = _organizationManagerRepository.GetById(request.OrganizationManagerID);
                organizationManager!.IsVerified = false;

                request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.IsApproved = false;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = true;
                request.IsDisable = true;
                result = true;
                notification.Content = "Yêu cầu trở thành tài khoản quản lý được xác minh của bạn chưa được công nhận, hãy cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
            }



            _createOrganizationManagerRequestRepository.Update(request);
            _organizationManagerRepository.Update(organizationManager);
            _accountRepository.Update(account);
            _notificationRepository.Save(notification);
            return result;
        }

        public void UpdateStatusCreateOrganizationManagerVerifiedRequest(UpdateStatusCreateOrganizationManagerVerifiedRequest request)
        {
            var createRequest = _createOrganizationManagerRequestRepository.GetById(request.CreateOrganizationManagerRequestID);
            if (createRequest == null)
            {
                throw new NotFoundException("Đơn này không tìm thấy!");
            }

            if (createRequest != null)
            {
                if (createRequest.IsApproved)
                    throw new BadRequestException(
                        "Đơn này đã được duyệt! Vì vậy mọi thông tin của đơn này hiện không thể chỉnh sửa!");

                if (request.IsDisable)
                {
                    createRequest.IsDisable = true;
                }

            }
        }

        private void TryValidateUpdateCreateOrganizationManagerRequest(UpdateCreateOrganizationManagerVerifiedAccountRequest request)
        {
            if (String.IsNullOrEmpty(request.CreateOrganizationManagerRequestID.ToString()))
            {
                throw new Exception("Id must not be null or empty!");
            }
            if (String.IsNullOrEmpty(request.IsApproved.ToString()))
            {
                throw new Exception("Trạng thái không được để trống!");
            }
        }
    }
}
