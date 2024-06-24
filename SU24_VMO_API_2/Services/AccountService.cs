using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Org.BouncyCastle.Asn1.Ocsp;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.JWTAuthSupport;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API.Supporters.Utils;
using SU24_VMO_API_2.DTOs.Request.AccountRequest;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;


namespace SU24_VMO_API.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountTokenRepository _accountTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IRequestManagerRepository _requestManagerRepository;
        private readonly JwtTokenSupporter jwtTokenSupporter;
        private readonly FirebaseService firebaseService;


        public AccountService(JwtTokenSupporter jwtTokenSupporter, IUserRepository _userRepository,
            IAdminRepository adminRepository, IOrganizationManagerRepository organizationManagerRepository, IRequestManagerRepository requestManagerRepository,
            IAccountRepository accountRepository, IAccountTokenRepository accountTokenRepository, FirebaseService firebaseService)
        {
            this.jwtTokenSupporter = jwtTokenSupporter;
            this._userRepository = _userRepository;
            _adminRepository = adminRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _requestManagerRepository = requestManagerRepository;
            _accountRepository = accountRepository;
            _accountTokenRepository = accountTokenRepository;
            this.firebaseService = firebaseService;
        }

        public IEnumerable<Account> GetAll()
        {
            return _accountRepository.GetAll();
        }

        public IEnumerable<Account> GetAllAccountsWithMemberRole()
        {
            return _accountRepository.GetAllAccountsWithMemberRole();
        }
        public IEnumerable<Account> GetAllAccountWithUserRole()
        {
            return _accountRepository.GetAllAccountsWithUserRole();
        }
        public IEnumerable<Account> GetAllAccountsWithRequestManagerRole()
        {
            return _accountRepository.GetAllAccountsWithRequestManagerRole();
        }
        public IEnumerable<Account> GetAllAccountsWithOrganizationManagerRole()
        {
            return _accountRepository.GetAllAccountsWithOrganizationManagerRole();
        }


        public Account? GetByAccountId(Guid accountId)
        {
            var account = _accountRepository.GetById(accountId);
            return account;
        }

        public Account? CreateAccount(CreateAccountRequest request)
        {
            TryValidateRegisterRequest(request);
            PasswordUtils.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var account = new Account
            {
                AccountID = Guid.NewGuid(),
                Username = request.Username,
                HashPassword = passwordHash,
                SaltPassword = passwordSalt,
                Role = request.Role,
                Avatar = request.Avatar,
                Email = request.Email,
                CreatedAt = TimeHelper.GetTime(DateTime.UtcNow),
                IsActived = true,
                IsBlocked = false,
            };

            if (account.Role.Equals(Role.Admin))
            {
                var admin = new Admin
                {
                    AccountID = account.AccountID,
                    AdminID = Guid.NewGuid(),
                    FirstName = "",
                    LastName = "",
                    PhoneNumber = ""
                };
                var accountCreated = _accountRepository.Save(account);
                var adminCreated = _adminRepository.Save(admin);
                return accountCreated;
            }
            else if (account.Role.Equals(Role.RequestManager))
            {
                var requestmanager = new RequestManager
                {
                    AccountID = account.AccountID,
                    RequestManagerID = Guid.NewGuid(),
                    FirstName = "",
                    LastName = "",
                    PhoneNumber = ""
                };
                var accountCreated = _accountRepository.Save(account);
                var requestManagerCreated = _requestManagerRepository.Save(requestmanager);
                return accountCreated;
            }
            else if (account.Role.Equals(Role.Member))
            {
                var member = new User
                {
                    AccountID = account.AccountID,
                    UserID = Guid.NewGuid(),
                    Gender = "",
                    FirstName = "",
                    LastName = "",
                    PhoneNumber = ""
                };
                var accountCreated = _accountRepository.Save(account);
                var memberCreated = _userRepository.Save(member);
                return accountCreated;
            }
            else if (account.Role.Equals(Role.User))
            {
                var user = new User
                {
                    AccountID = account.AccountID,
                    UserID = Guid.NewGuid(),
                    Gender = "",
                    FirstName = "",
                    LastName = "",
                    PhoneNumber = ""
                };
                var accountCreated = _accountRepository.Save(account);
                var userCreated = _userRepository.Save(user);
                return accountCreated;
            }
            else if (account.Role.Equals(Role.OrganizationManager))
            {
                var om = new OrganizationManager
                {
                    AccountID = account.AccountID,
                    OrganizationManagerID = Guid.NewGuid(),
                    Gender = "",
                    FirstName = "",
                    LastName = "",
                    PhoneNumber = ""
                };
                var accountCreated = _accountRepository.Save(account);
                var organizationManagerCreated = _organizationManagerRepository.Save(om);
                return accountCreated;
            }
            else
            {
                return null;
            }

        }

        public void UpdateAccount(UpdateAccountRequest request)
        {
            TryValidateUpdateRequest(request);
            var account = _accountRepository.GetById(request.AccountID)!;

            var user = new User();
            var om = new OrganizationManager();
            if (account.Role == Role.User || account.Role == Role.Member)
            {
                user = _userRepository.GetByAccountId(account.AccountID)!;
                if (!String.IsNullOrEmpty(request.FirstName))
                {
                    user.FirstName = request.FirstName;
                }
                if (!String.IsNullOrEmpty(request.LastName))
                {
                    user.LastName = request.LastName;
                }
                if (!String.IsNullOrEmpty(request.PhoneNumber))
                {
                    user.PhoneNumber = request.PhoneNumber;
                }
                if (!String.IsNullOrEmpty(request.BirthDay.ToString()))
                {
                    user.BirthDay = request.BirthDay;
                }
                if (!String.IsNullOrEmpty(request.Gender))
                {
                    user.Gender = request.Gender;
                }
                if (!String.IsNullOrEmpty(request.FacebookUrl))
                {
                    user.FacebookUrl = request.FacebookUrl;
                }
                if (!String.IsNullOrEmpty(request.YoutubeUrl))
                {
                    user.YoutubeUrl = request.YoutubeUrl;
                }
                if (!String.IsNullOrEmpty(request.TiktokUrl))
                {
                    user.TiktokUrl = request.TiktokUrl;
                }
                account.ModifiedBy = user.UserID;
                _accountRepository.Update(account);
                _userRepository.Update(user);
            }
            else if (account.Role == Role.OrganizationManager)
            {
                om = _organizationManagerRepository.GetByAccountID(account.AccountID)!;
                if (!String.IsNullOrEmpty(request.FirstName))
                {
                    om.FirstName = request.FirstName;
                }
                if (!String.IsNullOrEmpty(request.LastName))
                {
                    om.LastName = request.LastName;
                }
                if (!String.IsNullOrEmpty(request.PhoneNumber))
                {
                    om.PhoneNumber = request.PhoneNumber;
                }
                if (!String.IsNullOrEmpty(request.BirthDay.ToString()))
                {
                    om.BirthDay = (DateTime)request.BirthDay!;
                }
                if (!String.IsNullOrEmpty(request.Gender))
                {
                    om.Gender = request.Gender;
                }
                if (!String.IsNullOrEmpty(request.FacebookUrl))
                {
                    om.FacebookUrl = request.FacebookUrl;
                }
                if (!String.IsNullOrEmpty(request.YoutubeUrl))
                {
                    om.YoutubeUrl = request.YoutubeUrl;
                }
                if (!String.IsNullOrEmpty(request.TiktokUrl))
                {
                    om.TiktokUrl = request.TiktokUrl;
                }
                account.ModifiedBy = om.OrganizationManagerID;
                _accountRepository.Update(account);
                _organizationManagerRepository.Update(om);
            }
            else
            {
                return;
            }
        }

        //function login
        public (string?, string?) Login(string? methodAllowed, string password)
        {
            if (!String.IsNullOrEmpty(methodAllowed))
            {
                var account = _accountRepository.GetByEmail(methodAllowed);
                if (account != null && account.IsBlocked == false)
                {
                    if (PasswordUtils.VerifyPasswordHash(password, account.HashPassword, account.SaltPassword))
                    {

                        if (account.Role.Equals(Role.Admin))
                        {
                            var admin = _adminRepository.GetByAccountID(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateTokenForAdmin(admin);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshTokenForAdmin(admin);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.User))
                        {
                            var user = _userRepository.GetByAccountId(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateToken(user);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshToken(user);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.Member))
                        {
                            var user = _userRepository.GetByAccountId(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateToken(user);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshToken(user);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.OrganizationManager))
                        {
                            var organizationManager = _organizationManagerRepository.GetByAccountID(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateTokenForOrganizationManager(organizationManager);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshTokenForOrganizationManager(organizationManager);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.RequestManager))
                        {
                            var requestManager = _requestManagerRepository.GetByAccountID(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateTokenForRequestManager(requestManager);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshTokenForRequestManager(requestManager);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }
                    }
                }
                account = _accountRepository.GetByUsername(methodAllowed);
                if (account != null && account.IsBlocked == false)
                {
                    if (PasswordUtils.VerifyPasswordHash(password, account.HashPassword, account.SaltPassword))
                    {

                        if (account.Role.Equals(Role.Admin))
                        {
                            var admin = _adminRepository.GetByAccountID(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateTokenForAdmin(admin);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshTokenForAdmin(admin);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.User))
                        {
                            var user = _userRepository.GetByAccountId(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateToken(user);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshToken(user);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.Member))
                        {
                            var user = _userRepository.GetByAccountId(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateToken(user);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshToken(user);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.OrganizationManager))
                        {
                            var organizationManager = _organizationManagerRepository.GetByAccountID(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateTokenForOrganizationManager(organizationManager);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshTokenForOrganizationManager(organizationManager);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.RequestManager))
                        {
                            var requestManager = _requestManagerRepository.GetByAccountID(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateTokenForRequestManager(requestManager);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshTokenForRequestManager(requestManager);
                            var accountToken = new AccountToken
                            {
                                AccountTokenId = Guid.NewGuid(),
                                AccountID = account.AccountID,
                                AccessToken = accessToken,
                                CodeRefreshToken = code,
                                RefreshToken = refreshToken,
                                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                                ExpiredDateAccessToken = expiredAccessDate,
                                ExpiredDateRefreshToken = expiredRefreshDate
                            };
                            _accountTokenRepository.Save(accountToken);
                            _accountRepository.Update(account);
                            //UpdateTokenForUser(user, token);
                            return (accessToken, refreshToken);
                        }
                    }
                }
            }
            return (null, null);
        }



        //validation refresh token 
        public (string?, string?) ValidateRefreshToken(string refreshTokenCheck)
        {
            return jwtTokenSupporter.ValidateRefreshToken(refreshTokenCheck);
        }


        //function update avatar cho user
        public async Task<string?> UpdateAvatar(Guid accountId, IFormFile file)
        {
            var account = _accountRepository.GetById(accountId);
            if (account != null)
            {
                var link = await firebaseService.UploadImage(file);
                if (link != null)
                {
                    account.Avatar = link;
                    account.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
                    _accountRepository.Update(account);
                }
                return link;
            }
            return null;
        }


        //reset pasword
        public bool? ResetPassword(ResetPasswordRequest request)
        {
            TryValidateResetPasswordRequest(request);
            var account = _accountRepository.GetByEmail(request.Email.Trim());
            if (account == null) return null;
            PasswordUtils.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            account.HashPassword = passwordHash;
            account.SaltPassword = passwordSalt;
            account.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
            _accountRepository.Update(account);
            return true;
        }

        public bool? CheckPassword(CheckPasswordRequest request)
        {
            var account = _accountRepository.GetByEmail(request.Email.Trim());
            if (account == null) throw new NotFoundException("Email is not valid.");
            if (PasswordUtils.VerifyPasswordHash(request.OldPassword, account.HashPassword, account.SaltPassword))
            {
                return true;
            }
            return false;
        }

        private void TryValidateResetPasswordRequest(ResetPasswordRequest request)
        {
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new BadRequestException("Email is not valid.");
            }
        }

        private void TryValidateRegisterRequest(CreateAccountRequest request)
        {
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new BadRequestException("Email is not valid.");
            }
            if (_accountRepository.GetByEmail(request.Email) != null)
            {
                throw new BadRequestException("Email was existed!");
            }
            if (_accountRepository.GetByUsername(request.Username) != null)
            {
                throw new BadRequestException("Username was existed!");
            }
        }
        private void TryValidateUpdateRequest(UpdateAccountRequest request)
        {
            var account = _accountRepository.GetById(request.AccountID);
            if (account == null)
            {
                throw new NotFoundException("Account does not exist!");
            }

            if (!String.IsNullOrEmpty(request.PhoneNumber))
            {
                if (account.Role == Role.Member || account.Role == Role.User)
                {
                    var user = _userRepository.GetByAccountId(request.AccountID)!;
                    if (user.PhoneNumber != null && user.PhoneNumber.Equals(request.PhoneNumber) && IsPhoneNumberExisted(request.PhoneNumber))
                    {
                        throw new BadRequestException("Phone number already exists!");
                    }
                }

                if (account.Role == Role.OrganizationManager)
                {
                    var om = _organizationManagerRepository.GetByAccountID(request.AccountID)!;
                    if (om.PhoneNumber != null && om.PhoneNumber.Equals(request.PhoneNumber) && IsPhoneNumberExisted(request.PhoneNumber))
                    {
                        throw new BadRequestException("Phone number already exists!");
                    }
                }
            }
        }

        // Helper method to check if the phone number exists in the system
        private bool IsPhoneNumberExisted(string phoneNumber)
        {
            var userWithPhone = _userRepository.GetByPhone(phoneNumber);
            if (userWithPhone != null)
            {
                return true;
            }

            var orgManagerWithPhone = _organizationManagerRepository.GetByPhone(phoneNumber);
            return orgManagerWithPhone != null;
        }

    }
}
