using BusinessObject.Enums;
using BusinessObject.Models;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API.Supporters.Utils;
using System.Text.RegularExpressions;

namespace SU24_VMO_API.Services
{
    public class ModeratorService
    {
        private readonly IModeratorRepository _requestManagerRepository;
        private readonly IAccountRepository _accountRepository;


        public ModeratorService(IModeratorRepository requestManagerRepository, IAccountRepository accountRepository)
        {
            _requestManagerRepository = requestManagerRepository;
            _accountRepository = accountRepository;
        }

        public Moderator? CreateModerator(CreateNewRequestManagerRequest request)
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
                Role = Role.Moderator,
                IsActived = true,
                IsBlocked = false,
                CreatedAt = TimeHelper.GetTime(DateTime.UtcNow),
                UpdatedAt = null,
            };

            var requestManager = new Moderator()
            {
                ModeratorID = Guid.NewGuid(),
                AccountID = account.AccountID,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Account = account,
            };

            var requestManagerCreated = _requestManagerRepository.Save(requestManager);
            return requestManagerCreated;
        }


        private void TryValidateRegisterRequest(CreateNewRequestManagerRequest request)
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
            if (_accountRepository.GetByUsername(request.Username) != null)
            {
                throw new Exception("Username was existed!");
            }
            if (_requestManagerRepository.GetByPhone(request.PhoneNumber) != null)
            {
                throw new Exception("Phone number was existed!");
            }
        }
    }
}
