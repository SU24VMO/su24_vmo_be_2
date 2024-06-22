using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.EmailSupporter;
using SU24_VMO_API.Supporters.JWTAuthSupport;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API.Supporters.Utils;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SU24_VMO_API.Services
{
    public class UserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly JwtTokenSupporter jwtTokenSupporter;
        private readonly INotificationRepository _notificationRepository;

        public UserService(JwtTokenSupporter jwtTokenSupporter, IUserRepository _userRepository, IAccountRepository accountRepository, INotificationRepository notificationRepository)
        {
            this.jwtTokenSupporter = jwtTokenSupporter;
            this._userRepository = _userRepository;
            this._accountRepository = accountRepository;
            _notificationRepository = notificationRepository;
        }

        public User? CreateUser(CreateUserRequest request)
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
                Role = Role.User,
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

            var user = new User()
            {
                UserID = Guid.NewGuid(),
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
            if(accountCreated != null)
            {
                var userCreated = _userRepository.Save(user);
                if (userCreated != null)
                {
                    _notificationRepository.Save(notification);
                }
            }
            return user;
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

        private void TryValidateRegisterRequest(CreateUserRequest request)
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
            if (_userRepository.GetByPhone(request.PhoneNumber) != null)
            {
                throw new Exception("Phone number was existed!");
            }
        }


        public void UpdateUser(Guid userId, UpdateUserRequest request)
        {
            try
            {
                var user = _userRepository.GetById(userId) ?? throw new Exception("User not existed");
                var account = _accountRepository.GetById(user.AccountID)!;

                if (request.Email != null && request.Email != account.Email)
                {
                    var existingEmailAccount = _accountRepository.GetByEmail(request.Email);
                    if (existingEmailAccount != null)
                    {
                        throw new Exception("Email already exists");
                    }
                    account.Email = request.Email;
                }
                if (request.Username != null && request.Username != account.Username)
                {
                    var existingUsernameAccount = _accountRepository.GetByUsername(request.Username);
                    if (existingUsernameAccount != null)
                    {
                        throw new Exception("Username already exists");
                    }
                    account.Username = request.Username;
                }
                if (request.FirstName != null)
                {
                    user.FirstName = request.FirstName;
                }
                if (request.LastName != null)
                {
                    user.LastName = request.LastName;
                }
                if (request.PhoneNumber != null)
                {
                    user.PhoneNumber = request.PhoneNumber;
                }
                if (request.Gender != null)
                {
                    user.Gender = request.Gender;
                }
                if (request.BirthDay != null)
                {
                    user.BirthDay = request.BirthDay;
                }
                if (request.FacebookUrl != null)
                {
                    user.FacebookUrl = request.FacebookUrl;
                }
                if (request.YoutubeUrl != null)
                {
                    user.YoutubeUrl = request.YoutubeUrl;
                }
                if (request.TiktokUrl != null)
                {
                    user.TiktokUrl = request.TiktokUrl;
                }

                account!.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
                account!.ModifiedBy = account!.AccountID;
                _userRepository.Update(user);
                _accountRepository.Update(account);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
