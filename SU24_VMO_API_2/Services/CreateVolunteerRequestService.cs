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
    public class CreateVolunteerRequestService
    {
        private readonly ICreateVolunteerRequestRepository _createVolunteerRequestRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly CampaignService _campaignService;
        private readonly INotificationRepository _notificationRepository;



        public CreateVolunteerRequestService(ICreateVolunteerRequestRepository createVolunteerRequestRepository, IMemberRepository memberRepository,
            IAccountRepository accountRepository, CampaignService campaignService, INotificationRepository notificationRepository)
        {
            _createVolunteerRequestRepository = createVolunteerRequestRepository;
            _memberRepository = memberRepository;
            _accountRepository = accountRepository;
            _campaignService = campaignService;
            _notificationRepository = notificationRepository;
        }



        public IEnumerable<CreateVolunteerRequest>? GetAllCreateVolunteerRequests()
        {
            var requests = _createVolunteerRequestRepository.GetAll();
            foreach (var request in requests)
            {
                if (request.Member != null)
                {
                    if (request.Member.CreateCampaignRequests != null)
                        request.Member.CreateCampaignRequests.Clear();
                    if (request.Member.CreatePostRequests != null)
                        request.Member.CreatePostRequests.Clear();
                    if (request.Member.CreateMemberVerifiedRequests != null)
                        request.Member.CreateMemberVerifiedRequests.Clear();
                }
                if (request.Moderator != null)
                {
                    if (request.Moderator.CreateCampaignRequests != null)
                        request.Moderator.CreateCampaignRequests.Clear();
                    if (request.Moderator.CreatePostRequests != null)
                        request.Moderator.CreatePostRequests.Clear();
                    if (request.Moderator.CreateVolunteerRequests != null)
                        request.Moderator.CreateVolunteerRequests.Clear();
                    if (request.Moderator.CreateActivityRequests != null)
                        request.Moderator.CreateActivityRequests.Clear();
                    if (request.Moderator.CreateOrganizationManagerRequests != null)
                        request.Moderator.CreateOrganizationManagerRequests.Clear();
                    if (request.Moderator.CreateOrganizationRequests != null)
                        request.Moderator.CreateOrganizationRequests.Clear();
                }
            }
            return requests;
        }

        public CreateVolunteerRequest? CreateVolunteerRequest(CreateVolunteerAccountRequest request)
        {
            TryValidateRegisterRequest(request);
            if (_memberRepository.GetById(request.MemberID) == null) return null;
            //PasswordUtils.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var createVolunteerRequest = new CreateVolunteerRequest
            {
                CreateVolunteerRequestID = Guid.NewGuid(),
                MemberID = request.MemberID,
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
                CreateBy = request.MemberID,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsAcceptTermOfUse = request.IsAcceptTermOfUse,
                IsApproved = false,
                IsPending = true,
                IsRejected = false,
                IsLocked = false,
            };

            var volunteer = _memberRepository.GetById(request.MemberID);
            var account = _accountRepository.GetById(volunteer!.AccountID);


            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account!.AccountID,
                Content = "Yêu cầu trở thành thành viên của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!",
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };


            var createVolunteerRequestCreated = _createVolunteerRequestRepository.Save(createVolunteerRequest);
            if (createVolunteerRequestCreated != null)
            {
                _notificationRepository.Save(notification);
            }

            return createVolunteerRequestCreated;
        }

        public bool AcceptOrRejectCreateVolunteerAccountRequest(UpdateCreateVolunteerAccountRequest updateVolunteerAccountRequest)
        {
            TryValidateUpdateCreateVolunteerRequest(updateVolunteerAccountRequest);
            var request = _createVolunteerRequestRepository.GetById((Guid)updateVolunteerAccountRequest.CreateVolunteerRequestID!);
            var volunteer = new Member();
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

            if (updateVolunteerAccountRequest.IsApproved == true)
            {
                volunteer = _memberRepository.GetById(request.MemberID);
                account = volunteer!.Account;
                volunteer!.IsVerified = true;

                request.ApprovedBy = updateVolunteerAccountRequest.ModeratorId;
                request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.IsApproved = true;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = false;
                result = true;
                account!.Role = Role.Volunteer;
                notification.AccountID = account!.AccountID;
                notification.Content = "Yêu cầu trở thành thành viên của bạn vừa được duyệt thành công, hãy trải nghiệm dịch vụ nhé!";
            }
            else
            {
                volunteer = _memberRepository.GetById(request.MemberID);
                account = volunteer!.Account;
                volunteer!.IsVerified = false;

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



            _createVolunteerRequestRepository.Update(request);
            _memberRepository.Update(volunteer);
            _accountRepository.Update(account);
            _notificationRepository.Save(notification);
            return result;
        }


        private void TryValidateRegisterRequest(CreateVolunteerAccountRequest request)
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


        private void TryValidateUpdateCreateVolunteerRequest(UpdateCreateVolunteerAccountRequest request)
        {
            if (String.IsNullOrEmpty(request.CreateVolunteerRequestID.ToString()))
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
