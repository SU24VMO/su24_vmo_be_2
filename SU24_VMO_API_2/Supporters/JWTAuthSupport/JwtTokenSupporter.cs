using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.IdentityModel.Tokens;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API_2.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SU24_VMO_API.Supporters.JWTAuthSupport
{
    public class JwtTokenSupporter
    {
        IConfiguration config;
        IMemberRepository _memberRepository;
        IAccountRepository accountRepository;
        IAccountTokenRepository accountTokenRepository;
        IAdminRepository _adminRepository;
        IOrganizationManagerRepository _organizationManagerRepository;
        IModeratorRepository _moderatorRepository;
        private readonly IPAddressService ipAddressService;



        public JwtTokenSupporter(IConfiguration config, IMemberRepository memberRepo, IAccountRepository accountRepository, IAccountTokenRepository accountTokenRepository,
            IAdminRepository adminRepository, IOrganizationManagerRepository organizationManagerRepository, IModeratorRepository moderatorRepository, IPAddressService ipAddressService)
        {
            this.config = config;
            _memberRepository = memberRepo;
            this.accountRepository = accountRepository;
            this.accountTokenRepository = accountTokenRepository;
            _adminRepository = adminRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _moderatorRepository = moderatorRepository;
            this.ipAddressService = ipAddressService;
        }

        public (string, DateTime?) CreateToken(Member user)
        {
            var account = accountRepository.GetById(user.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var code = Guid.NewGuid().ToString();
            //var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
            var key = GetValidBase64Key(config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).ToString()),
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim(ClaimTypes.SerialNumber, code),
                new Claim("member_id", user.MemberID.ToString()),
                new Claim("account_id", user.AccountID.ToString()),
                new Claim("username", String.IsNullOrEmpty(account!.Username) ? "" : account!.Username),
                new Claim("firstname", user.FirstName),
                new Claim("lastname", user.LastName),
                new Claim("avatar",  String.IsNullOrEmpty(account!.Avatar) ? "" : account!.Avatar),
                new Claim("gender", String.IsNullOrEmpty(user.Gender) ? "" : user.Gender),
                new Claim("birthday", user.BirthDay == null ? "" : user.BirthDay.ToString()!),
                new Claim("phonenumber", String.IsNullOrEmpty(user.PhoneNumber) ? "" : user.PhoneNumber),
                new Claim("facebooklink", String.IsNullOrEmpty(user.FacebookUrl) ? "" : user.FacebookUrl),
                new Claim("youtubelink", String.IsNullOrEmpty(user.YoutubeUrl) ? "" : user.YoutubeUrl),
                new Claim("tiktoklink", String.IsNullOrEmpty(user.TiktokUrl) ? "" : user.TiktokUrl),
                new Claim("email", account!.Email),
                new Claim("is_verified", user.IsVerified.ToString()),
                }),
                Audience = config["Jwt:Audience"],
                Issuer = config["Jwt:Issuer"],
                Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) { CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false } }
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), tokenDescriptor.Expires);
        }


        public (string, DateTime?) CreateTokenForAdmin(Admin admin)
        {
            var account = accountRepository.GetById(admin.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var code = Guid.NewGuid().ToString();
            //var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var key = GetValidBase64Key(config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).ToString()),
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim(ClaimTypes.SerialNumber, code),
                new Claim("admin_id", admin.AdminID.ToString()),
                new Claim("username", String.IsNullOrEmpty(account!.Username) ? "" : account!.Username),
                new Claim("firstname", admin.FirstName),
                new Claim("lastname", admin.LastName),
                new Claim("avatar",  String.IsNullOrEmpty(account!.Avatar) ? "" : account!.Avatar),
                new Claim("phonenumber", String.IsNullOrEmpty(admin.PhoneNumber) ? "" : admin.PhoneNumber),
                new Claim("email", account!.Email),
                new Claim("account_id", admin.AccountID.ToString())
            }),

                Audience = config["Jwt:Audience"],
                Issuer = config["Jwt:Issuer"],
                Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), tokenDescriptor.Expires);
        }


        public (string, DateTime?) CreateTokenForOrganizationManager(OrganizationManager organizationManager)
        {
            var account = accountRepository.GetById(organizationManager.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var code = Guid.NewGuid().ToString();
            //var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var key = GetValidBase64Key(config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).ToString()),
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim(ClaimTypes.SerialNumber, code),
                new Claim("organization_manager_id", organizationManager.OrganizationManagerID.ToString()),
                new Claim("account_id", organizationManager.AccountID.ToString()),
                new Claim("username", String.IsNullOrEmpty(account!.Username) ? "" : account!.Username),
                new Claim("firstname", organizationManager.FirstName),
                new Claim("lastname", organizationManager.LastName),
                new Claim("avatar",  String.IsNullOrEmpty(account!.Avatar) ? "" : account!.Avatar),
                new Claim("gender", String.IsNullOrEmpty(organizationManager.Gender) ? "" : organizationManager.Gender),
                new Claim("birthday", organizationManager.BirthDay.ToString()!),
                new Claim("phonenumber", String.IsNullOrEmpty(organizationManager.PhoneNumber) ? "" : organizationManager.PhoneNumber),
                new Claim("facebooklink", String.IsNullOrEmpty(organizationManager.FacebookUrl) ? "" : organizationManager.FacebookUrl),
                new Claim("youtubelink", String.IsNullOrEmpty(organizationManager.YoutubeUrl) ? "" : organizationManager.YoutubeUrl),
                new Claim("tiktoklink", String.IsNullOrEmpty(organizationManager.TiktokUrl) ? "" : organizationManager.TiktokUrl),
                new Claim("email", account!.Email),
                new Claim("is_verified", organizationManager.IsVerified.ToString()),
            }),

                Audience = config["Jwt:Audience"],
                Issuer = config["Jwt:Issuer"],
                Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), tokenDescriptor.Expires);
        }


        public (string, DateTime?) CreateTokenForModerator(Moderator requestManager)
        {
            var account = accountRepository.GetById(requestManager.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var code = Guid.NewGuid().ToString();
            //var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var key = GetValidBase64Key(config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).ToString()),
                new Claim(ClaimTypes.SerialNumber, code),
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim("moderator_id", requestManager.ModeratorID.ToString()),
                new Claim("account_id", requestManager.AccountID.ToString()),
                new Claim("username", String.IsNullOrEmpty(account!.Username) ? "" : account!.Username),
                new Claim("firstname", requestManager.FirstName),
                new Claim("lastname", requestManager.LastName),
                new Claim("avatar",  String.IsNullOrEmpty(account!.Avatar) ? "" : account!.Avatar),
                new Claim("phonenumber", String.IsNullOrEmpty(requestManager.PhoneNumber) ? "" : requestManager.PhoneNumber),
                new Claim("email", account!.Email),
            }),

                Audience = config["Jwt:Audience"],
                Issuer = config["Jwt:Issuer"],
                Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), tokenDescriptor.Expires);
        }


        //create refresh token for all role

        public (string, string, DateTime?) CreateRefreshToken(Member user)
        {
            var account = accountRepository.GetById(user.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
            var key = GetValidBase64Key(config["Jwt:Key"]!);
            var code = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).ToString()),
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim(ClaimTypes.SerialNumber, code), // id cho refresh token
                new Claim("member_id", user.MemberID.ToString()),
                new Claim("account_id", user.AccountID.ToString()),
                new Claim("username", String.IsNullOrEmpty(account!.Username) ? "" : account!.Username),
                new Claim("firstname", user.FirstName),
                new Claim("lastname", user.LastName),
                new Claim("avatar", account!.Avatar == null ? "" : account!.Avatar),
                new Claim("gender", String.IsNullOrEmpty(user.Gender) ? "" : user.Gender),
                new Claim("birthday", user.BirthDay == null ? "" : user.BirthDay.ToString()!),
                new Claim("phonenumber", String.IsNullOrEmpty(user.PhoneNumber) ? "" : user.PhoneNumber),
                new Claim("facebooklink", String.IsNullOrEmpty(user.FacebookUrl) ? "" : user.FacebookUrl),
                new Claim("youtubelink", String.IsNullOrEmpty(user.YoutubeUrl) ? "" : user.YoutubeUrl),
                new Claim("tiktoklink", String.IsNullOrEmpty(user.TiktokUrl) ? "" : user.TiktokUrl),
                new Claim("email", account!.Email),
                new Claim("is_verified", user.IsVerified.ToString()),
                }),
                Audience = config["Jwt:Audience"],
                Issuer = config["Jwt:Issuer"],
                //Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (code, tokenHandler.WriteToken(token), /*tokenDescriptor.Expires*/ null);
        }


        public (string, string, DateTime?) CreateRefreshTokenForAdmin(Admin admin)
        {
            var account = accountRepository.GetById(admin.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var key = GetValidBase64Key(config["Jwt:Key"]!);
            var code = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).ToString()),
                new Claim(ClaimTypes.SerialNumber, code), // id cho refresh token
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim("admin_id", admin.AdminID.ToString()),
                new Claim("account_id", admin.AccountID.ToString()),
                new Claim("username", String.IsNullOrEmpty(account!.Username) ? "" : account!.Username),
                new Claim("firstname", admin.FirstName),
                new Claim("lastname", admin.LastName),
                new Claim("avatar",  String.IsNullOrEmpty(account!.Avatar) ? "" : account!.Avatar),
                new Claim("phonenumber", String.IsNullOrEmpty(admin.PhoneNumber) ? "" : admin.PhoneNumber),
                new Claim("email", account!.Email),
            }),

                Audience = config["Jwt:Audience"],
                Issuer = config["Jwt:Issuer"],
                //Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (code, tokenHandler.WriteToken(token), /*tokenDescriptor.Expires*/ null);
        }


        public (string, string, DateTime?) CreateRefreshTokenForOrganizationManager(OrganizationManager organizationManager)
        {
            var account = accountRepository.GetById(organizationManager.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var key = GetValidBase64Key(config["Jwt:Key"]!);
            var code = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).ToString()),
                new Claim(ClaimTypes.SerialNumber, code), // id cho refresh token
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim("organization_manager_id", organizationManager.OrganizationManagerID.ToString()),
                new Claim("account_id", organizationManager.AccountID.ToString()),
                new Claim("username", String.IsNullOrEmpty(account!.Username) ? "" : account!.Username),
                new Claim("firstname", organizationManager.FirstName),
                new Claim("lastname", organizationManager.LastName),
                new Claim("avatar",  String.IsNullOrEmpty(account!.Avatar) ? "" : account!.Avatar),
                new Claim("gender", String.IsNullOrEmpty(organizationManager.Gender) ? "" : organizationManager.Gender),
                new Claim("birthday", organizationManager.BirthDay.ToString()!),
                new Claim("phonenumber", String.IsNullOrEmpty(organizationManager.PhoneNumber) ? "" : organizationManager.PhoneNumber),
                new Claim("facebooklink", String.IsNullOrEmpty(organizationManager.FacebookUrl) ? "" : organizationManager.FacebookUrl),
                new Claim("youtubelink", String.IsNullOrEmpty(organizationManager.YoutubeUrl) ? "" : organizationManager.YoutubeUrl),
                new Claim("tiktoklink", String.IsNullOrEmpty(organizationManager.TiktokUrl) ? "" : organizationManager.TiktokUrl),
                new Claim("email", account!.Email),
                new Claim("is_verified", organizationManager.IsVerified.ToString()),
            }),

                Audience = config["Jwt:Audience"],
                Issuer = config["Jwt:Issuer"],
                //Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (code, tokenHandler.WriteToken(token), /*tokenDescriptor.Expires*/ null);
        }





        public (string, string, DateTime?) CreateRefreshTokenForModerator(Moderator moderator)
        {
            var account = accountRepository.GetById(moderator.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var key = GetValidBase64Key(config["Jwt:Key"]!);
            var code = Guid.NewGuid().ToString();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).ToString()),
                new Claim(ClaimTypes.SerialNumber, code), // id cho refresh token
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim("moderator_id", moderator.ModeratorID.ToString()),
                new Claim("account_id", moderator.AccountID.ToString()),
                new Claim("username", String.IsNullOrEmpty(account!.Username) ? "" : account!.Username),
                new Claim("firstname", moderator.FirstName),
                new Claim("lastname", moderator.LastName),
                new Claim("avatar",  String.IsNullOrEmpty(account!.Avatar) ? "" : account!.Avatar),
                new Claim("phonenumber", String.IsNullOrEmpty(moderator.PhoneNumber) ? "" : moderator.PhoneNumber),
                new Claim("email", account!.Email),
            }),

                Audience = config["Jwt:Audience"],
                Issuer = config["Jwt:Issuer"],
                //Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (code, tokenHandler.WriteToken(token), /*tokenDescriptor.Expires*/ null);
        }





        public Member? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                //var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
                var key = GetValidBase64Key(config["Jwt:Key"]!);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var memberId = jwtToken.Claims.First(claim => claim.Type == "id").Value;
                var id = Guid.Parse(memberId);
                var member = _memberRepository.GetById(id);
                return member;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public Member? ExtractUserFromRequestToken(HttpContext context)
        {
            var token = context.Request.Headers.Authorization.ToString().Split(" ").Last();
            if (token == null)
            {
                return null;
            }
            return ValidateToken(token);
        }



        public (string?, string?) ValidateRefreshToken(string refreshTokenCheck)
        {
            try
            {
                //var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
                var key = GetValidBase64Key(config["Jwt:Key"]!);
                var claimPrinciple = new JwtSecurityTokenHandler().ValidateToken(
                         refreshTokenCheck,
                         new TokenValidationParameters
                         {
                             IssuerSigningKey = new SymmetricSecurityKey(key),
                             ValidateIssuer = true,
                             ValidateAudience = true,
                             ValidateLifetime = false,
                             ValidIssuer = config["Jwt:Issuer"],
                             ValidAudience = config["Jwt:Audience"],
                             ValidateIssuerSigningKey = true,
                             ClockSkew = TimeSpan.Zero
                         },
                         out _);
                if (claimPrinciple == null) return (null, null);
                string? serialNumber = claimPrinciple.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
                if (serialNumber == null) return (null, null);
                var accountToken = accountTokenRepository.CheckRefreshToken(serialNumber);
                if (accountToken != null)
                {
                    Account? account = accountRepository.GetById(accountToken.AccountID);
                    if (account.Role.Equals(Role.Admin))
                    {
                        var admin = _adminRepository.GetByAccountID(account.AccountID)!;
                        var (token, expiredAccessDate) = CreateTokenForAdmin(admin);

                        var accessToken = token;
                        var (code, refreshToken, expiredRefreshDate) = CreateRefreshTokenForAdmin(admin);
                        var accountTokenWhenRefresh = new AccountToken
                        {
                            AccountTokenId = Guid.NewGuid(),
                            AccountID = account.AccountID,
                            AccessToken = accessToken,
                            CodeRefreshToken = code,
                            RefreshToken = refreshToken,
                            CreatedDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            ExpiredDateAccessToken = expiredAccessDate,
                            ExpiredDateRefreshToken = expiredRefreshDate
                        };
                        accountTokenRepository.Save(accountTokenWhenRefresh);
                        accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
                        ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                        {
                            IPAddressId = Guid.NewGuid(),
                            AccountId = account.AccountID,
                            CreateDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            LoginTime = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            IPAddressValue = GetLocalIPAddress()
                        });
                        return (accessToken, refreshToken);
                    }

                    if (account.Role.Equals(Role.Member))
                    {
                        var member = _memberRepository.GetByAccountId(account.AccountID)!;
                        var (token, expiredAccessDate) = CreateToken(member);

                        var accessToken = token;
                        var (code, refreshToken, expiredRefreshDate) = CreateRefreshToken(member);
                        var accountTokenWhenRefresh = new AccountToken
                        {
                            AccountTokenId = Guid.NewGuid(),
                            AccountID = account.AccountID,
                            AccessToken = accessToken,
                            CodeRefreshToken = code,
                            RefreshToken = refreshToken,
                            CreatedDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            ExpiredDateAccessToken = expiredAccessDate,
                            ExpiredDateRefreshToken = expiredRefreshDate
                        };
                        accountTokenRepository.Save(accountTokenWhenRefresh);
                        accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
                        ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                        {
                            IPAddressId = Guid.NewGuid(),
                            AccountId = account.AccountID,
                            CreateDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            LoginTime = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            IPAddressValue = GetLocalIPAddress()
                        });
                        return (accessToken, refreshToken);
                    }

                    if (account.Role.Equals(Role.Volunteer))
                    {
                        var member = _memberRepository.GetByAccountId(account.AccountID)!;
                        var (token, expiredAccessDate) = CreateToken(member);

                        var accessToken = token;
                        var (code, refreshToken, expiredRefreshDate) = CreateRefreshToken(member);
                        var accountTokenWhenRefresh = new AccountToken
                        {
                            AccountTokenId = Guid.NewGuid(),
                            AccountID = account.AccountID,
                            AccessToken = accessToken,
                            CodeRefreshToken = code,
                            RefreshToken = refreshToken,
                            CreatedDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            ExpiredDateAccessToken = expiredAccessDate,
                            ExpiredDateRefreshToken = expiredRefreshDate
                        };
                        accountTokenRepository.Save(accountTokenWhenRefresh);
                        accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
                        ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                        {
                            IPAddressId = Guid.NewGuid(),
                            AccountId = account.AccountID,
                            CreateDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            LoginTime = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            IPAddressValue = GetLocalIPAddress()
                        });
                        return (accessToken, refreshToken);
                    }

                    if (account.Role.Equals(Role.OrganizationManager))
                    {
                        var organizationManager = _organizationManagerRepository.GetByAccountID(account.AccountID)!;
                        var (token, expiredAccessDate) = CreateTokenForOrganizationManager(organizationManager);

                        var accessToken = token;
                        var (code, refreshToken, expiredRefreshDate) = CreateRefreshTokenForOrganizationManager(organizationManager);
                        var accountTokenWhenRefresh = new AccountToken
                        {
                            AccountTokenId = Guid.NewGuid(),
                            AccountID = account.AccountID,
                            AccessToken = accessToken,
                            CodeRefreshToken = code,
                            RefreshToken = refreshToken,
                            CreatedDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            ExpiredDateAccessToken = expiredAccessDate,
                            ExpiredDateRefreshToken = expiredRefreshDate
                        };
                        accountTokenRepository.Save(accountTokenWhenRefresh);
                        accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
                        ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                        {
                            IPAddressId = Guid.NewGuid(),
                            AccountId = account.AccountID,
                            CreateDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            LoginTime = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            IPAddressValue = GetLocalIPAddress()
                        });
                        return (accessToken, refreshToken);
                    }

                    if (account.Role.Equals(Role.Moderator))
                    {
                        var moderator = _moderatorRepository.GetByAccountID(account.AccountID)!;
                        var (token, expiredAccessDate) = CreateTokenForModerator(moderator);

                        var accessToken = token;
                        var (code, refreshToken, expiredRefreshDate) = CreateRefreshTokenForModerator(moderator);
                        var accountTokenWhenRefresh = new AccountToken
                        {
                            AccountTokenId = Guid.NewGuid(),
                            AccountID = account.AccountID,
                            AccessToken = accessToken,
                            CodeRefreshToken = code,
                            RefreshToken = refreshToken,
                            CreatedDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            ExpiredDateAccessToken = expiredAccessDate,
                            ExpiredDateRefreshToken = expiredRefreshDate
                        };
                        accountTokenRepository.Save(accountTokenWhenRefresh);
                        accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
                        ipAddressService.CreateIpAddress(new BusinessObject.Models.IPAddress
                        {
                            IPAddressId = Guid.NewGuid(),
                            AccountId = account.AccountID,
                            CreateDate = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            LoginTime = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow),
                            IPAddressValue = GetLocalIPAddress()
                        });
                        return (accessToken, refreshToken);
                    }
                }
                return (null, null);
            }
            catch (Exception)
            {
                return (null, null);
            }
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

        private byte[] GetValidBase64Key(string base64Key)
        {
            try
            {
                return Encoding.UTF8.GetBytes(base64Key);
            }
            catch (FormatException)
            {
                throw new Exception("The JWT key in the configuration is not a valid Base64 string.");
            }
        }


        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, // You might want to validate the audience and issuer as well
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                ValidateLifetime = false // Here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

    }
}
