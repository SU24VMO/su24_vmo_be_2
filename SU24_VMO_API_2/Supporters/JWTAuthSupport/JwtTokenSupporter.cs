using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.IdentityModel.Tokens;
using Repository.Implements;
using Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SU24_VMO_API.Supporters.JWTAuthSupport
{
    public class JwtTokenSupporter
    {
        IConfiguration config;
        IUserRepository _userRepository;
        IAccountRepository accountRepository;
        IAccountTokenRepository accountTokenRepository;
        IAdminRepository _adminRepository;
        IOrganizationManagerRepository _organizationManagerRepository;
        IRequestManagerRepository _requestManagerRepository;


        public JwtTokenSupporter(IConfiguration config, IUserRepository userRepo, IAccountRepository accountRepository, IAccountTokenRepository accountTokenRepository, 
            IAdminRepository adminRepository, IOrganizationManagerRepository organizationManagerRepository, IRequestManagerRepository requestManagerRepository)
        {
            this.config = config;
            this._userRepository = userRepo;
            this.accountRepository = accountRepository;
            this.accountTokenRepository = accountTokenRepository;
            _adminRepository = adminRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _requestManagerRepository = requestManagerRepository;
        }

        public (string, DateTime?) CreateToken(User user)
        {
            var account = accountRepository.GetById(user.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var code = Guid.NewGuid().ToString();
            var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim(ClaimTypes.SerialNumber, code),
                new Claim("user_id", user.UserID.ToString()),
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
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), tokenDescriptor.Expires);
        }


        public (string, DateTime?) CreateTokenForAdmin(Admin admin)
        {
            var account = accountRepository.GetById(admin.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var code = Guid.NewGuid().ToString();
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
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
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
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


        public (string, DateTime?) CreateTokenForRequestManager(RequestManager requestManager)
        {
            var account = accountRepository.GetById(requestManager.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var code = Guid.NewGuid().ToString();   
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.SerialNumber, code),
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim("request_manager_id", requestManager.RequestManagerID.ToString()),
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

        public (string, string, DateTime?) CreateRefreshToken(User user)
        {
            var account = accountRepository.GetById(user.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
            var code = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim(ClaimTypes.SerialNumber, code), // id cho refresh token
                new Claim("user_id", user.UserID.ToString()),
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
                Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (code, tokenHandler.WriteToken(token), tokenDescriptor.Expires);
        }


        public (string, string, DateTime?) CreateRefreshTokenForAdmin(Admin admin)
        {
            var account = accountRepository.GetById(admin.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var code = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
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
                Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (code, tokenHandler.WriteToken(token), tokenDescriptor.Expires);
        }


        public (string, string, DateTime?) CreateRefreshTokenForOrganizationManager(OrganizationManager organizationManager)
        {
            var account = accountRepository.GetById(organizationManager.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var code = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
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
                Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (code, tokenHandler.WriteToken(token), tokenDescriptor.Expires);
        }





        public (string, string, DateTime?) CreateRefreshTokenForRequestManager(RequestManager requestManager)
        {
            var account = accountRepository.GetById(requestManager.AccountID);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
            var code = Guid.NewGuid().ToString();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.SerialNumber, code), // id cho refresh token
                new Claim(ClaimTypes.Role, account!.Role.ToString()),
                new Claim("request_manager_id", requestManager.RequestManagerID.ToString()),
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
                Expires = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (code, tokenHandler.WriteToken(token), tokenDescriptor.Expires);
        }





        public User? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(claim => claim.Type == "id").Value;
                var id = Guid.Parse(userId);
                var user = _userRepository.GetById(id);
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public User? ExtractUserFromRequestToken(HttpContext context)
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
                var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
                var claimPrinciple = new JwtSecurityTokenHandler().ValidateToken(
                         refreshTokenCheck,
                         new TokenValidationParameters
                         {
                             IssuerSigningKey = new SymmetricSecurityKey(key),
                             ValidateIssuer = false,
                             ValidateAudience = false,
                             ValidateLifetime = false,
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
                        //var accountToken = new AccountToken
                        //{
                        //    AccountTokenId = Guid.NewGuid(),
                        //    AccountID = account.AccountID,
                        //    AccessToken = accessToken,
                        //    CodeRefreshToken = code,
                        //    RefreshToken = refreshToken,
                        //    CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                        //    ExpiredDateAccessToken = expiredAccessDate,
                        //    ExpiredDateRefreshToken = expiredRefreshDate
                        //};
                        //_accountTokenRepository.Save(accountToken);
                        //_accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
                        return (accessToken, refreshToken);
                    }

                    if (account.Role.Equals(Role.User))
                    {
                        var user = _userRepository.GetByAccountId(account.AccountID)!;
                        var (token, expiredAccessDate) = CreateToken(user);

                        var accessToken = token;
                        var (code, refreshToken, expiredRefreshDate) = CreateRefreshToken(user);
                        //var accountToken = new AccountToken
                        //{
                        //    AccountTokenId = Guid.NewGuid(),
                        //    AccountID = account.AccountID,
                        //    AccessToken = accessToken,
                        //    CodeRefreshToken = code,
                        //    RefreshToken = refreshToken,
                        //    CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                        //    ExpiredDateAccessToken = expiredAccessDate,
                        //    ExpiredDateRefreshToken = expiredRefreshDate
                        //};
                        //_accountTokenRepository.Save(accountToken);
                        //_accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
                        return (accessToken, refreshToken);
                    }

                    if (account.Role.Equals(Role.Member))
                    {
                        var user = _userRepository.GetByAccountId(account.AccountID)!;
                        var (token, expiredAccessDate) = CreateToken(user);

                        var accessToken = token;
                        var (code, refreshToken, expiredRefreshDate) = CreateRefreshToken(user);
                        //var accountToken = new AccountToken
                        //{
                        //    AccountTokenId = Guid.NewGuid(),
                        //    AccountID = account.AccountID,
                        //    AccessToken = accessToken,
                        //    CodeRefreshToken = code,
                        //    RefreshToken = refreshToken,
                        //    CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                        //    ExpiredDateAccessToken = expiredAccessDate,
                        //    ExpiredDateRefreshToken = expiredRefreshDate
                        //};
                        //_accountTokenRepository.Save(accountToken);
                        //_accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
                        return (accessToken, refreshToken);
                    }

                    if (account.Role.Equals(Role.OrganizationManager))
                    {
                        var organizationManager = _organizationManagerRepository.GetByAccountID(account.AccountID)!;
                        var (token, expiredAccessDate) = CreateTokenForOrganizationManager(organizationManager);

                        var accessToken = token;
                        var (code, refreshToken, expiredRefreshDate) = CreateRefreshTokenForOrganizationManager(organizationManager);
                        //var accountToken = new AccountToken
                        //{
                        //    AccountTokenId = Guid.NewGuid(),
                        //    AccountID = account.AccountID,
                        //    AccessToken = accessToken,
                        //    CodeRefreshToken = code,
                        //    RefreshToken = refreshToken,
                        //    CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                        //    ExpiredDateAccessToken = expiredAccessDate,
                        //    ExpiredDateRefreshToken = expiredRefreshDate
                        //};
                        //_accountTokenRepository.Save(accountToken);
                        //_accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
                        return (accessToken, refreshToken);
                    }

                    if (account.Role.Equals(Role.RequestManager))
                    {
                        var requestManager = _requestManagerRepository.GetByAccountID(account.AccountID)!;
                        var (token, expiredAccessDate) = CreateTokenForRequestManager(requestManager);

                        var accessToken = token;
                        var (code, refreshToken, expiredRefreshDate) = CreateRefreshTokenForRequestManager(requestManager);
                        //var accountToken = new AccountToken
                        //{
                        //    AccountTokenId = Guid.NewGuid(),
                        //    AccountID = account.AccountID,
                        //    AccessToken = accessToken,
                        //    CodeRefreshToken = code,
                        //    RefreshToken = refreshToken,
                        //    CreatedDate = TimeHelper.GetTime(DateTime.UtcNow),
                        //    ExpiredDateAccessToken = expiredAccessDate,
                        //    ExpiredDateRefreshToken = expiredRefreshDate
                        //};
                        //_accountTokenRepository.Save(accountToken);
                        //_accountRepository.Update(account);
                        //UpdateTokenForUser(user, token);
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
    }
}
