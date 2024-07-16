using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Org.BouncyCastle.Asn1.Ocsp;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.EmailSupporter;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.JWTAuthSupport;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API.Supporters.Utils;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SU24_VMO_API.Services
{
    public class MemberService
    {

        private readonly IMemberRepository _memberRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly JwtTokenSupporter _jwtTokenSupporter;
        private readonly INotificationRepository _notificationRepository;

        public MemberService(JwtTokenSupporter jwtTokenSupporter, IMemberRepository memberRepository, IAccountRepository accountRepository,
            INotificationRepository notificationRepository)
        {
            _jwtTokenSupporter = jwtTokenSupporter;
            _memberRepository = memberRepository;
            _accountRepository = accountRepository;
            _notificationRepository = notificationRepository;
        }


        public IEnumerable<Member>? GetAllMembers()
        {
            return _memberRepository.GetAll();
        }


        public IEnumerable<Member> GetMembersByMemberName(string memberName)
        {
            var members = _memberRepository.GetAll().Where(m => (m.FirstName.Trim().ToLower() + " " + m.LastName.Trim().ToLower()).Contains(memberName.ToLower().Trim()));
            return members;
        }


        public Member? CreateMember(CreateMemberRequest request)
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
                Role = Role.Member,
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

            var member = new Member()
            {
                MemberID = Guid.NewGuid(),
                AccountID = account.AccountID,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                BirthDay = request.BirthDay,
                FacebookUrl = request.FacebookUrl,
                TiktokUrl = request.TiktokUrl,
                YoutubeUrl = request.YoutubeUrl,
                IsVerified = false
            };


            var accountCreated = _accountRepository.Save(account);
            if (accountCreated != null)
            {
                var userCreated = _memberRepository.Save(member);
                if (userCreated != null)
                {
                    _notificationRepository.Save(notification);
                }
            }
            return member;
        }


        public string? ChangePasswordInCaseForgot(string email)
        {
            var account = _accountRepository.GetByEmail(email);
            if (account == null)
            {
                return null;
            }
            return EmailSupporter.SendEmailForResetPassword(email);
        }

        public async Task<string?> SendOTPWhenCreateNewUser(CreateMemberRequest request)
        {
            TryValidateRegisterRequest(request);
            return await EmailSupporter.SendOTPForCreateNewUser(request.Email);
        }


        public static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-=_+";

            using RandomNumberGenerator rng = RandomNumberGenerator.Create();

            byte[] data = new byte[length];
            rng.GetBytes(data);

            // Convert bytes to characters
            char[] password = new char[length];
            for (int i = 0; i < length; i++)
            {
                password[i] = chars[data[i] % chars.Length];
            }

            return new string(password);
        }

        private void TryValidateRegisterRequest(CreateMemberRequest request)
        {
            if (new Regex(RegexCollector.PhoneRegex).IsMatch(request.PhoneNumber) == false)
            {
                throw new Exception("Số điện thoại không hợp lệ!");
            }
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new Exception("Email không đúng định dạng!");
            }

            if (string.IsNullOrEmpty(request.FirstName))
            {
                throw new Exception("Họ không được để trống!");
            }
            if (string.IsNullOrEmpty(request.LastName))
            {
                throw new Exception("Tên không được để trống!");
            }
            if (string.IsNullOrEmpty(request.Gender))
            {
                throw new Exception("Giới tính không được để trống");
            }
            if (_accountRepository.GetByUsername(request.Username) != null)
            {
                throw new Exception("Tên người dùng đã tồn tại!");
            }
            if (_memberRepository.GetByPhone(request.PhoneNumber) != null)
            {
                throw new Exception("Số điện thoại đã tồn tại!");
            }

            if (TimeHelper.GetTime(DateTime.UtcNow).Year - request.BirthDay.Year < 18)
            {
                throw new Exception("Người dùng phải từ 18 tuổi trở lên!");
            }
        }


        public void UpdateMember(Guid memberId, UpdateMemberRequest request)
        {
            try
            {
                var member = _memberRepository.GetById(memberId) ?? throw new Exception("Thành viên không tồn tại!");
                var account = _accountRepository.GetById(member.AccountID)!;

                if (request.Email != null && request.Email != account.Email)
                {
                    var existingEmailAccount = _accountRepository.GetByEmail(request.Email);
                    if (existingEmailAccount != null)
                    {
                        throw new Exception("Email đã tồn tại");
                    }
                    account.Email = request.Email;
                }
                if (request.Username != null && request.Username != account.Username)
                {
                    var existingUsernameAccount = _accountRepository.GetByUsername(request.Username);
                    if (existingUsernameAccount != null)
                    {
                        throw new Exception("Tên người dùng đã tồn tại!");
                    }
                    account.Username = request.Username;
                }
                if (request.FirstName != null)
                {
                    member.FirstName = request.FirstName;
                }
                if (request.LastName != null)
                {
                    member.LastName = request.LastName;
                }
                if (request.PhoneNumber != null)
                {
                    member.PhoneNumber = request.PhoneNumber;
                }
                if (request.Gender != null)
                {
                    member.Gender = request.Gender;
                }
                if (request.BirthDay != null)
                {
                    member.BirthDay = request.BirthDay;
                }
                if (request.FacebookUrl != null)
                {
                    member.FacebookUrl = request.FacebookUrl;
                }
                if (request.YoutubeUrl != null)
                {
                    member.YoutubeUrl = request.YoutubeUrl;
                }
                if (request.TiktokUrl != null)
                {
                    member.TiktokUrl = request.TiktokUrl;
                }

                account!.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
                account!.ModifiedBy = account!.AccountID;
                _memberRepository.Update(member);
                _accountRepository.Update(account);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
