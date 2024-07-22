using BusinessObject.Enums;
using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API.Supporters.Utils;
using SU24_VMO_API_2.DTOs.Request;
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



        public IEnumerable<CreateVolunteerRequest> GetAllCreateVolunteerRequests()
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

        public CreateVolunteerRequest? GetCreateVolunteerRequestById(Guid createVolunteerRequestId)
        {
            return GetAllCreateVolunteerRequests().FirstOrDefault(c => c.CreateVolunteerRequestID.Equals(createVolunteerRequestId));
        }

        public IEnumerable<CreateVolunteerRequest>? GetAllCreateVolunteerRequestsByMemberName(string? memberName)
        {
            if (!String.IsNullOrEmpty(memberName))
                return GetAllCreateVolunteerRequests().Where(m => (m.Member!.FirstName.Trim().ToLower() + " " + m.Member.LastName.Trim().ToLower()).Contains(memberName.ToLower().Trim()));
            else return GetAllCreateVolunteerRequests();
        }



        public CreateVolunteerRequest? CreateVolunteerRequest(CreateVolunteerAccountRequest request)
        {
            TryValidateRegisterRequest(request);
            if (_memberRepository.GetById(request.MemberID) == null) return null;

            //validation cho email
            var createVolunteerRequestExisted = _createVolunteerRequestRepository.GetCreateVolunteerRequestsWithEmail(request.Email);
            if (createVolunteerRequestExisted != null && createVolunteerRequestExisted.Count() > 0)
            {
                var listRequestPending = createVolunteerRequestExisted.Where(c => c.IsPending == true);
                if (listRequestPending.Count() > 0)
                {
                    throw new BadRequestException("Đơn tạo tài khoản tình nguyện viên cho tài khoản với email này có vẻ như đã được tạo, vui lòng đợi hệ thống phản hồi!");
                }

                var listRequestApproved = createVolunteerRequestExisted.Where(c => c.IsApproved == true && c.CreateBy.Equals(request.MemberID));
                if (listRequestApproved.Count() > 0)
                {
                    throw new BadRequestException("Tài khoản này hiện đã trở thành tài khoản tình nguyện viên!");
                }
            }


            if (createVolunteerRequestExisted != null && createVolunteerRequestExisted.Count() > 0 && createVolunteerRequestExisted.Where(c => c.IsApproved == true).Count() > 0)
            {
                throw new BadRequestException("Email này hiện đã được sử dụng! Vui lòng sử dụng email khác!");
            }

            //validation cho phone number
            var createVolunteerRequestExistedWithPhoneNumber = _createVolunteerRequestRepository.GetCreateVolunteerRequestsWithPhoneNumber(request.PhoneNumber);
            if (createVolunteerRequestExistedWithPhoneNumber != null && createVolunteerRequestExistedWithPhoneNumber.Count() > 0)
            {
                var listRequestPending = createVolunteerRequestExistedWithPhoneNumber.Where(c => c.IsPending == true);
                if (listRequestPending.Count() > 0)
                {
                    throw new BadRequestException("Đơn tạo tài khoản tình nguyện viên cho tài khoản với số điện thoại này có vẻ như đã được tạo, vui lòng đợi hệ thống phản hồi!");
                }

                var listRequestApproved = createVolunteerRequestExistedWithPhoneNumber.Where(c => c.IsApproved == true && c.CreateBy.Equals(request.MemberID));
                if (listRequestApproved.Count() > 0)
                {
                    throw new BadRequestException("Tài khoản này hiện đã trở thành tài khoản tình nguyện viên!");
                }
            }


            if (createVolunteerRequestExistedWithPhoneNumber != null && createVolunteerRequestExistedWithPhoneNumber.Count() > 0 && createVolunteerRequestExistedWithPhoneNumber.Where(c => c.IsApproved == true).Count() > 0)
            {
                throw new BadRequestException("Số điện thoại này hiện đã được sử dụng! Vui lòng sử dụng số điện thoại khác!");
            }

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
                CitizenIdentification = request.CitizenIdentification,
                DetailDescriptionLink = request.DetailDescriptionLink,
                AchievementLink = request.AchievementLink,
                CreateBy = request.MemberID,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsAcceptTermOfUse = request.IsAcceptTermOfUse,
                IsApproved = false,
                IsPending = true,
                IsRejected = false,
                IsLocked = false,
                IsDisable = false
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

        public void UpdateCreateVolunteerRequest(Guid createVolunteerRequestId, UpdateCreateVolunteerRequestRequest request)
        {
            var requestExisted =
                _createVolunteerRequestRepository.GetById(createVolunteerRequestId);
            if (requestExisted == null)
            {
                throw new NotFoundException("Đơn duyệt này không tìm thấy!");
            }

            if (requestExisted.IsApproved)
            {
                throw new BadRequestException("Đơn tạo tài khoản thành viên xác thực này hiện đã được duyệt, vì vậy mọi thông tin về đơn này hiện không thể chỉnh sửa!");
            }

            if (!String.IsNullOrEmpty(request.SocialMediaLink))
            {
                requestExisted.SocialMediaLink = request.SocialMediaLink;
            }

            if (!String.IsNullOrEmpty(request.CitizenIdentification))
            {
                requestExisted.CitizenIdentification = request.CitizenIdentification;
            }

            if (!String.IsNullOrEmpty(request.Email))
            {
                requestExisted.Email = request.Email;
            }
            if (!String.IsNullOrEmpty(request.PhoneNumber))
            {
                requestExisted.PhoneNumber = request.PhoneNumber;
            }
            if (!String.IsNullOrEmpty(request.AchievementLink))
            {
                requestExisted.AchievementLink = request.AchievementLink;
            }
            if (!String.IsNullOrEmpty(request.DetailDescriptionLink))
            {
                requestExisted.DetailDescriptionLink = request.DetailDescriptionLink;
            }

            if (!String.IsNullOrEmpty(request.ClubName))
            {
                requestExisted.ClubName = request.ClubName;
            }
            if (!String.IsNullOrEmpty(request.MemberAddress))
            {
                requestExisted.MemberAddress = request.MemberAddress;
            }
            if (!String.IsNullOrEmpty(request.MemberName))
            {
                requestExisted.MemberName = request.MemberName;
            }
            if (request.IsAcceptTermOfUse != null)
            {
                requestExisted.IsAcceptTermOfUse = (bool)request.IsAcceptTermOfUse;
            }
            if (request.Birthday != null)
            {
                requestExisted.Birthday = (DateTime)request.Birthday;
            }
            if (request.RoleInClub != null)
            {
                requestExisted.RoleInClub = request.RoleInClub;
            }

            requestExisted.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);

            _createVolunteerRequestRepository.Update(requestExisted);
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
                //request.IsDisable = false;
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

                request.ApprovedBy = updateVolunteerAccountRequest.ModeratorId;
                request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.IsApproved = false;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = true;
                //request.IsDisable = true;
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




        public void UpdateStatusCreateVolunteerAccountRequest(UpdateStatusCreateVolunteerAccountRequest request)
        {
            var createRequest = _createVolunteerRequestRepository.GetById(request.CreateVolunteerRequestID);
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


        private void TryValidateRegisterRequest(CreateVolunteerAccountRequest request)
        {
            if (new Regex(RegexCollector.PhoneRegex).IsMatch(request.PhoneNumber) == false)
            {
                throw new BadRequestException("Số điện thoại không hợp lệ");
            }
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new BadRequestException("Email không hợp lệ.");
            }
        }


        private void TryValidateUpdateCreateVolunteerRequest(UpdateCreateVolunteerAccountRequest request)
        {
            if (String.IsNullOrEmpty(request.CreateVolunteerRequestID.ToString()))
            {
                throw new BadRequestException("Id must not be null or empty!");
            }
            if (String.IsNullOrEmpty(request.IsApproved.ToString()))
            {
                throw new BadRequestException("Trạng thái không được để trống!");
            }
        }

    }
}
