using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.JWTAuthSupport;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API.Supporters.Utils;
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
            return _accountRepository.GetById(accountId);
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
            var account = _accountRepository.GetById(request.AccountID);
            if (account == null)
            {
                throw new Exception("Account is not existed!");
            }

            if (!String.IsNullOrEmpty(request.Username))
            {
                account.Username = request.Username;
            }
            if (!String.IsNullOrEmpty(request.Avatar))
            {
                account.Avatar = request.Avatar;
            }
            if (!String.IsNullOrEmpty(request.IsActived.ToString()))
            {
                account.IsActived = request.IsActived;
            }
            if (!String.IsNullOrEmpty(request.IsBlocked.ToString()))
            {
                account.IsBlocked = request.IsBlocked;
            }

            _accountRepository.Update(account);
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
        private void TryValidateResetPasswordRequest(ResetPasswordRequest request)
        {
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new Exception("Email is not valid.");
            }
        }

        private void TryValidateRegisterRequest(CreateAccountRequest request)
        {
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new Exception("Email is not valid.");
            }
            if (_accountRepository.GetByEmail(request.Email) != null)
            {
                throw new Exception("Email was existed!");
            }
            if (_accountRepository.GetByUsername(request.Username) != null)
            {
                throw new Exception("Username was existed!");
            }
        }
        private void TryValidateUpdateRequest(UpdateAccountRequest request)
        {
            if (!String.IsNullOrEmpty(request.Username) && _accountRepository.GetByUsername(request.Username) != null)
            {
                throw new Exception("Username was existed!");
            }
        }
    }
}
