using BusinessObject.Enums;
using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API.Supporters.Utils;
using System.Text.RegularExpressions;


namespace SU24_VMO_API.Services
{
    public class CreateMemberRequestService
    {
        private readonly ICreateMemberRequestRepository _createMemberRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly CampaignService _campaignService;
        private readonly INotificationRepository _notificationRepository;



        public CreateMemberRequestService(ICreateMemberRequestRepository createMemberRequestRepository, IUserRepository userRepository,
            IAccountRepository accountRepository, CampaignService campaignService, INotificationRepository notificationRepository)
        {
            _createMemberRequestRepository = createMemberRequestRepository;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _campaignService = campaignService;
            _notificationRepository = notificationRepository;
        }



        public IEnumerable<CreateMemberRequest>? GetAllCreateMemberRequests()
        {
            var requests = _createMemberRequestRepository.GetAll();
            foreach (var request in requests)
            {
                if (request.User != null)
                {
                    if (request.User.CreateCampaignRequests != null)
                        request.User.CreateCampaignRequests.Clear();
                    if (request.User.CreatePostRequests != null)
                        request.User.CreatePostRequests.Clear();
                    if (request.User.CreateUserVerifiedRequests != null)
                        request.User.CreateUserVerifiedRequests.Clear();
                }
                if (request.RequestManager != null)
                {
                    if (request.RequestManager.CreateCampaignRequests != null)
                        request.RequestManager.CreateCampaignRequests.Clear();
                    if (request.RequestManager.CreatePostRequests != null)
                        request.RequestManager.CreatePostRequests.Clear();
                    if (request.RequestManager.CreateMemberRequests != null)
                        request.RequestManager.CreateMemberRequests.Clear();
                    if (request.RequestManager.CreateActivityRequests != null)
                        request.RequestManager.CreateActivityRequests.Clear();
                    if (request.RequestManager.CreateOrganizationManagerRequests != null)
                        request.RequestManager.CreateOrganizationManagerRequests.Clear();
                    if (request.RequestManager.CreateOrganizationRequests != null)
                        request.RequestManager.CreateOrganizationRequests.Clear();
                }
            }
            return requests;
        }

        public CreateMemberRequest? CreateMemberRequest(CreateMemberAccountRequest request)
        {
            TryValidateRegisterRequest(request);
            if (_userRepository.GetById(request.UserID) == null) return null;
            //PasswordUtils.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var createMemberRequest = new CreateMemberRequest
            {
                CreateMemberRequestID = Guid.NewGuid(),
                UserID = request.UserID,
                MemberName = request.MemberName,
                Birthday = request.Birthday,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                SocialMediaLink = request.SocialMediaLink,
                MemberAddress = request.MemberAddress,
                RoleInClub = request.RoleInClub,
                ClubName = request.ClubName,
                DetailDescriptionLink = request.DetailDescriptionLink,
                AchievementLink = request.AchievementLink,
                CreateBy = request.UserID,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsAcceptTermOfUse = request.IsAcceptTermOfUse,
                IsApproved = false,
                IsPending = true,
                IsRejected = false,
                IsLocked = false,
            };

            var user = _userRepository.GetById(request.UserID);
            var account = _accountRepository.GetById(user!.AccountID);


            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account!.AccountID,
                Content = "Yêu cầu trở thành thành viên của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };


            var createMemberRequestCreated = _createMemberRequestRepository.Save(createMemberRequest);
            if (createMemberRequestCreated != null)
            {
                _notificationRepository.Save(notification);
            }

            return createMemberRequestCreated;
        }

        public bool AcceptOrRejectCreateMemberAccountRequest(UpdateCreateMemberAccountRequest updateMemberAccountRequest)
        {
            TryValidateUpdateCreateMemberRequest(updateMemberAccountRequest);
            var request = _createMemberRequestRepository.GetById((Guid)updateMemberAccountRequest.CreateMemberRequestID!);
            var user = new User();
            var account = new Account();
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

            if (updateMemberAccountRequest.IsApproved == true)
            {
                user = _userRepository.GetById(request.UserID);
                account = user!.Account;
                user!.IsVerified = true;

                request.ApprovedBy = updateMemberAccountRequest.RequestManagerId;
                request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.IsApproved = true;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = false;
                result = true;
                account!.Role = Role.Member;
                notification.AccountID = account!.AccountID;
                notification.Content = "Yêu cầu trở thành thành viên của bạn vừa được duyệt thành công, hãy trải nghiệm dịch vụ nhé!";
            }
            else
            {
                user = _userRepository.GetById(request.UserID);
                account = user!.Account;
                user!.IsVerified = false;

                request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.IsApproved = false;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = true;
                result = true;
                account!.Role = Role.Member;
                notification.AccountID = account!.AccountID;
                notification.Content = "Yêu cầu trở thành thành viên của bạn chưa được công nhận, hãy cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
            }



            _createMemberRequestRepository.Update(request);
            _userRepository.Update(user);
            _accountRepository.Update(account);
            _notificationRepository.Save(notification);
            return result;
        }


        private void TryValidateRegisterRequest(CreateMemberAccountRequest request)
        {
            if (new Regex(RegexCollector.PhoneRegex).IsMatch(request.PhoneNumber) == false)
            {
                throw new Exception("Phone is not a valid phone");
            }
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new Exception("Email is not valid.");
            }
        }


        private void TryValidateUpdateCreateMemberRequest(UpdateCreateMemberAccountRequest request)
        {
            if (String.IsNullOrEmpty(request.CreateMemberRequestID.ToString()))
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
