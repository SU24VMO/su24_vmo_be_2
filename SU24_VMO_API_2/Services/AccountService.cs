using BusinessObject.Enums;
using BusinessObject.Models;
using MailKit;
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
using SU24_VMO_API_2.DTOs.Response;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using SU24_VMO_API_2.Services;
using System.Net.Sockets;
using System.Net;


namespace SU24_VMO_API.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountTokenRepository _accountTokenRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IModeratorRepository _moderatorRepository;
        private readonly CampaignService _campaignService;
        private readonly JwtTokenSupporter jwtTokenSupporter;
        private readonly FirebaseService firebaseService;
        private readonly IPAddressService ipAddressService;


        public AccountService(JwtTokenSupporter jwtTokenSupporter, IMemberRepository memberRepository,
            IAdminRepository adminRepository, IOrganizationManagerRepository organizationManagerRepository, IModeratorRepository moderatorRepository,
            IAccountRepository accountRepository, IAccountTokenRepository accountTokenRepository, FirebaseService firebaseService,
            CampaignService campaignService, ITransactionRepository transactionRepository, IPAddressService ipAddressService)
        {
            this.jwtTokenSupporter = jwtTokenSupporter;
            _memberRepository = memberRepository;
            _adminRepository = adminRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _moderatorRepository = moderatorRepository;
            _accountRepository = accountRepository;
            _accountTokenRepository = accountTokenRepository;
            this.firebaseService = firebaseService;
            _campaignService = campaignService;
            _transactionRepository = transactionRepository;
            this.ipAddressService = ipAddressService;
        }

        public IEnumerable<Account> GetAll()
        {
            return _accountRepository.GetAll();
        }

        public IEnumerable<Account> GetAllActiveStatus()
        {
            return _accountRepository.GetAll().Where(a => a.IsActived == true);
        }


        public int CalculateNumberOfAccountDonate()
        {
            var accounts = _accountRepository.GetAll();
            int count = 0;
            foreach (var account in accounts)
            {
                var transactions = _transactionRepository.GetHistoryTransactionByAccountId(account.AccountID);
                if (transactions != null && transactions.Any())
                {
                    var transactionsSuccess = transactions.Where(t =>
                        t.TransactionType == TransactionType.Receive &&
                        t.TransactionStatus == TransactionStatus.Success);
                    if (transactionsSuccess.Any())
                        count++;
                }
            }
            return count;
        }

        public int CalculateNumberOfAccountUser()
        {
            var accounts = _accountRepository.GetAll();
            int count = accounts.Count();
            return count;
        }

        public IEnumerable<Account> GetAllAccountsWithVolunteerRole(string? name)
        {
            if (!String.IsNullOrEmpty(name))
                return _accountRepository.GetAllAccountsWithVolunteerRole().Where(a => a.Username.ToLower().Contains(name.ToLower().Trim()));
            else
                return _accountRepository.GetAllAccountsWithVolunteerRole();
        }
        public IEnumerable<Account> GetAllAccountWithMemberRole(string? name)
        {
            if (!String.IsNullOrEmpty(name))
                return _accountRepository.GetAllAccountWithMemberRole().Where(a => a.Username.ToLower().Contains(name.ToLower().Trim()));
            else
                return _accountRepository.GetAllAccountWithMemberRole();
        }
        public IEnumerable<Account> GetAllAccountsWithModeratorRole(string? name)
        {
            if (!String.IsNullOrEmpty(name))
                return _accountRepository.GetAllAccountsWithModeratorRole().Where(a => a.Username.ToLower().Contains(name.ToLower().Trim()));
            else
                return _accountRepository.GetAllAccountsWithModeratorRole();

        }
        public IEnumerable<Account> GetAllAccountsWithOrganizationManagerRole(string? name)
        {
            if (!String.IsNullOrEmpty(name))
                return _accountRepository.GetAllAccountsWithOrganizationManagerRole().Where(a => a.Username.ToLower().Contains(name.ToLower().Trim()));
            else
                return _accountRepository.GetAllAccountsWithOrganizationManagerRole();

        }


        public Account? GetByAccountId(Guid accountId)
        {
            var account = _accountRepository.GetById(accountId);
            return account;
        }

        public GetByAccountIdResponse? GetByAccountIdResponse(Guid accountId)
        {
            var account = _accountRepository.GetById(accountId);
            if (account == null) { throw new NotFoundException("Tài khoản không tồn tại!"); }
            float donatedMoney = 0;
            int numberOfDonation = 0;
            int numberOfActiveCampaign = 0;

            if (account != null && account.Transactions != null)
            {
                foreach (var transaction in account.Transactions)
                {
                    if (transaction.TransactionStatus == TransactionStatus.Success)
                    {
                        donatedMoney += transaction.Amount;
                        numberOfDonation++;
                    }
                }
            }

            var response = new GetByAccountIdResponse
            {
                AccountID = accountId,
                Avatar = account!.Avatar,
                AccountTokens = account.AccountTokens,
                BankingAccounts = account.BankingAccounts,
                CreatedAt = account.CreatedAt,
                DonatedMoney = donatedMoney,
                Email = account.Email,
                HashPassword = account.HashPassword,
                IsActived = account.IsActived,
                IsBlocked = account.IsBlocked,
                ModifiedBy = account.ModifiedBy,
                Notifications = account.Notifications,
                NumberOfDonations = numberOfDonation,
                Role = account.Role,
                SaltPassword = account.SaltPassword,
                Transactions = account.Transactions,
                UpdatedAt = account.UpdatedAt,
                Username = account.Username
            };

            if (response.Notifications != null)
            {
                foreach (var notification in response.Notifications)
                {
                    notification.Account = null;
                }
            }

            if (response.BankingAccounts != null)
            {
                foreach (var bankingAccount in response.BankingAccounts)
                {
                    bankingAccount.Account = null;
                }
            }

            if (response.Transactions != null)
            {
                foreach (var transaction in response.Transactions)
                {
                    transaction.Account = null;
                }
            }

            if (account.Role == Role.Member)
            {
                var member = _memberRepository.GetByAccountId(accountId);
                if (member != null)
                {
                    response.FirstName = member.FirstName;
                    response.LastName = member.LastName;
                    response.LinkFacebook = member.FacebookUrl;
                    response.LinkYoutube = member.YoutubeUrl;
                    response.LinkTiktok = member.TiktokUrl;
                    response.IsVerified = false;

                    response.Campaigns = _campaignService.GetAllCampaignByCreateByVolunteerId(member.MemberID, null).ToList();
                    response.NumberOfActiveCampaign = _campaignService.GetAllCampaignByCreateByVolunteerId(member.MemberID, null).ToList().Where(c => c.IsActive == true).Count();
                    if (member.IsVerified)
                        response.IsVerified = true;

                }

            }
            if (account.Role == Role.Volunteer)
            {
                var member = _memberRepository.GetByAccountId(accountId);
                if (member != null)
                {
                    response.FirstName = member.FirstName;
                    response.LastName = member.LastName;
                    response.LinkFacebook = member.FacebookUrl;
                    response.LinkYoutube = member.YoutubeUrl;
                    response.LinkTiktok = member.TiktokUrl;
                    response.IsVerified = false;

                    response.Campaigns = _campaignService.GetAllCampaignByCreateByVolunteerId(member.MemberID, null).ToList();
                    response.NumberOfActiveCampaign = _campaignService.GetAllCampaignByCreateByVolunteerId(member.MemberID, null).ToList().Where(c => c.IsActive == true).Count();
                    if (member.IsVerified)
                        response.IsVerified = true;
                }
            }
            if (account.Role == Role.OrganizationManager)
            {
                var om = _organizationManagerRepository.GetByAccountID(accountId);
                if (om != null)
                {
                    response.FirstName = om.FirstName;
                    response.LastName = om.LastName;
                    response.LinkFacebook = om.FacebookUrl;
                    response.LinkYoutube = om.YoutubeUrl;
                    response.LinkTiktok = om.TiktokUrl;
                    response.IsVerified = false;


                    response.Campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerId(om.OrganizationManagerID, null).ToList();
                    response.NumberOfActiveCampaign = _campaignService.GetAllCampaignByCreateByOrganizationManagerId(om.OrganizationManagerID, null).ToList().Where(c => c.IsActive == true).Count();
                    if (om.IsVerified)
                        response.IsVerified = true;
                }
            }

            if (account.Role == Role.Moderator)
            {
                var moderator = _moderatorRepository.GetByAccountID(accountId);
                if (moderator != null)
                {
                    response.FirstName = moderator.FirstName;
                    response.LastName = moderator.LastName;
                }
            }

            if (account.Role == Role.Admin)
            {
                var admin = _adminRepository.GetByAccountID(accountId);
                if (admin != null)
                {
                    response.FirstName = admin.FirstName;
                    response.LastName = admin.LastName;
                }
            }

            return response;
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
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = ""
                };
                var accountCreated = _accountRepository.Save(account);
                var adminCreated = _adminRepository.Save(admin);
                return accountCreated;
            }
            else if (account.Role.Equals(Role.Moderator))
            {
                var moderator = new Moderator
                {
                    AccountID = account.AccountID,
                    ModeratorID = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = ""
                };
                var accountCreated = _accountRepository.Save(account);
                var moderatorCreated = _moderatorRepository.Save(moderator);
                return accountCreated;
            }
            else if (account.Role.Equals(Role.Member))
            {
                var member = new Member
                {
                    AccountID = account.AccountID,
                    MemberID = Guid.NewGuid(),
                    Gender = "",
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = ""
                };
                var accountCreated = _accountRepository.Save(account);
                var memberCreated = _memberRepository.Save(member);
                return accountCreated;
            }
            else if (account.Role.Equals(Role.Member))
            {
                var member = new Member
                {
                    AccountID = account.AccountID,
                    MemberID = Guid.NewGuid(),
                    Gender = "",
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = ""
                };
                var accountCreated = _accountRepository.Save(account);
                var memberCreated = _memberRepository.Save(member);
                return accountCreated;
            }
            else if (account.Role.Equals(Role.OrganizationManager))
            {
                var om = new OrganizationManager
                {
                    AccountID = account.AccountID,
                    OrganizationManagerID = Guid.NewGuid(),
                    Gender = "",
                    FirstName = request.FirstName,
                    LastName = request.LastName,
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

            var member = new Member();
            var om = new OrganizationManager();
            if (account.Role == Role.Volunteer || account.Role == Role.Member)
            {
                member = _memberRepository.GetByAccountId(account.AccountID)!;
                if (!String.IsNullOrEmpty(request.FirstName))
                {
                    member.FirstName = request.FirstName;
                }
                if (!String.IsNullOrEmpty(request.LastName))
                {
                    member.LastName = request.LastName;
                }
                if (!String.IsNullOrEmpty(request.PhoneNumber))
                {
                    member.PhoneNumber = request.PhoneNumber;
                }
                if (!String.IsNullOrEmpty(request.BirthDay.ToString()))
                {
                    member.BirthDay = request.BirthDay;
                }
                if (!String.IsNullOrEmpty(request.Gender))
                {
                    member.Gender = request.Gender;
                }
                if (!String.IsNullOrEmpty(request.FacebookUrl))
                {
                    member.FacebookUrl = request.FacebookUrl;
                }
                if (!String.IsNullOrEmpty(request.YoutubeUrl))
                {
                    member.YoutubeUrl = request.YoutubeUrl;
                }
                if (!String.IsNullOrEmpty(request.TiktokUrl))
                {
                    member.TiktokUrl = request.TiktokUrl;
                }
                account.ModifiedBy = member.MemberID;
                _accountRepository.Update(account);
                _memberRepository.Update(member);
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

        public void UpdateAccountStatus(UpdateAccountStatusRequest request)
        {
            var account = _accountRepository.GetById(request.AccountID)!;
            if (account == null) { throw new NotFoundException("Tài khoản không tìm thấy!"); }
            var member = new Member();
            var om = new OrganizationManager();
            var moderator = new Moderator();
            if (account.Role == Role.Volunteer || account.Role == Role.Member)
            {
                member = _memberRepository.GetByAccountId(account.AccountID)!;
                if (request.IsActived != null)
                {
                    account.IsActived = (bool)request.IsActived;
                    if ((bool)request.IsActived)
                    {
                        account.IsBlocked = false;
                    }
                }

                account.ModifiedBy = member.MemberID;
                _accountRepository.Update(account);
                _memberRepository.Update(member);
            }
            else if (account.Role == Role.OrganizationManager)
            {
                om = _organizationManagerRepository.GetByAccountID(account.AccountID)!;
                if (request.IsActived != null)
                {
                    account.IsActived = (bool)request.IsActived;
                    if ((bool)request.IsActived)
                    {
                        account.IsBlocked = false;
                    }
                }
                account.ModifiedBy = om.OrganizationManagerID;
                _accountRepository.Update(account);
                _organizationManagerRepository.Update(om);
            }
            else if (account.Role == Role.Moderator)
            {
                moderator = _moderatorRepository.GetByAccountID(account.AccountID)!;
                if (request.IsActived != null)
                {
                    account.IsActived = (bool)request.IsActived;
                    if ((bool)request.IsActived)
                    {
                        account.IsBlocked = false;
                    }
                }
                account.ModifiedBy = moderator.ModeratorID;
                _accountRepository.Update(account);
                _moderatorRepository.Update(moderator);
            }
            else
            {
                return;
            }
        }

        //function login
        public (string?, string?) Login(string? methodAllowed, string password, HttpContext httpContext, string? longitude, string? latitude, string? road, string? suburb, string? city, string? country, string? postCode, string? countryCode)
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.Member))
                        {
                            var member = _memberRepository.GetByAccountId(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateToken(member);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshToken(member);
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.Volunteer))
                        {
                            var member = _memberRepository.GetByAccountId(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateToken(member);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshToken(member);
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.Moderator))
                        {
                            var moderator = _moderatorRepository.GetByAccountID(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateTokenForModerator(moderator);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshTokenForModerator(moderator);
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.Member))
                        {
                            var member = _memberRepository.GetByAccountId(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateToken(member);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshToken(member);
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.Volunteer))
                        {
                            var member = _memberRepository.GetByAccountId(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateToken(member);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshToken(member);
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
                            return (accessToken, refreshToken);
                        }

                        if (account.Role.Equals(Role.Moderator))
                        {
                            var moderator = _moderatorRepository.GetByAccountID(account.AccountID)!;
                            var (token, expiredAccessDate) = jwtTokenSupporter.CreateTokenForModerator(moderator);

                            var accessToken = token;
                            var (code, refreshToken, expiredRefreshDate) = jwtTokenSupporter.CreateRefreshTokenForModerator(moderator);
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
                            ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                            {
                                IPAddressId = Guid.NewGuid(),
                                AccountId = account.AccountID,
                                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                                LoginTime = TimeHelper.GetTime(DateTime.UtcNow),
                                IPAddressValue = GetIpAddress(httpContext),
                                Longitude = longitude,
                                Latitude = latitude,
                                City = city,
                                Country = country,
                                CountryCode = countryCode,
                                Postcode = postCode,
                                Road = road,
                                Suburb = suburb
                            });
                            return (accessToken, refreshToken);
                        }
                    }
                }

                if (account.IsBlocked)
                {
                    throw new BadRequestException("Tài khoản hiện đã bị khóa, vui lòng liên hệ quản trị viên để mở khóa!");
                }
            }
            return (null, null);
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public string GetIpAddress(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            }

            return ip;
        }



        //validation refresh token 
        public (string?, string?) ValidateRefreshToken(string refreshTokenCheck, HttpContext httpContext, string? longitude, string? latitude, string? road, string? suburb, string? city, string? country, string? postCode, string? countryCode)
        {
            return jwtTokenSupporter.ValidateRefreshToken(refreshTokenCheck, httpContext, longitude, latitude, road, suburb, city, country, postCode, countryCode);
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
            if (account == null) throw new NotFoundException("Email không hợp lệ.");
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
                throw new BadRequestException("Email không hợp lệ.");
            }
        }

        private void TryValidateRegisterRequest(CreateAccountRequest request)
        {
            if (new Regex(RegexCollector.EmailRegex).IsMatch(request.Email) == false)
            {
                throw new BadRequestException("Email không hợp lệ.");
            }
            if (_accountRepository.GetByEmail(request.Email) != null)
            {
                throw new BadRequestException("Email đã tồn tại!");
            }
            if (_accountRepository.GetByUsername(request.Username) != null)
            {
                throw new BadRequestException("Tên người dùng đã tồn tại!");
            }
        }
        private void TryValidateUpdateRequest(UpdateAccountRequest request)
        {
            var account = _accountRepository.GetById(request.AccountID);
            if (account == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại!");
            }

            if (!String.IsNullOrEmpty(request.PhoneNumber))
            {
                if (account.Role == Role.Volunteer || account.Role == Role.Member)
                {
                    var user = _memberRepository.GetByAccountId(request.AccountID)!;
                    if (user.PhoneNumber != null && user.PhoneNumber.Equals(request.PhoneNumber) && IsPhoneNumberExisted(request.PhoneNumber))
                    {
                        throw new BadRequestException("Số điện thoại đã tồn tại!");
                    }
                }

                if (account.Role == Role.OrganizationManager)
                {
                    var om = _organizationManagerRepository.GetByAccountID(request.AccountID)!;
                    if (om.PhoneNumber != null && om.PhoneNumber.Equals(request.PhoneNumber) && IsPhoneNumberExisted(request.PhoneNumber))
                    {
                        throw new BadRequestException("Số điện thoại đã tồn tại!");
                    }
                }
            }
        }

        public ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            return jwtTokenSupporter.GetPrincipalFromExpiredToken(token);
        }

        // Helper method to check if the phone number exists in the system
        private bool IsPhoneNumberExisted(string phoneNumber)
        {
            var userWithPhone = _memberRepository.GetByPhone(phoneNumber);
            if (userWithPhone != null)
            {
                return true;
            }

            var orgManagerWithPhone = _organizationManagerRepository.GetByPhone(phoneNumber);
            return orgManagerWithPhone != null;
        }

    }
}
