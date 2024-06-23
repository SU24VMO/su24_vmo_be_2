using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using Microsoft.AspNetCore.Authorization;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Attributes;

namespace SU24_VMO_API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly UserService _userService;
        private readonly OrganizationManagerService _organizationManagerService;
        private readonly RequestManagerService _requestManagerService;





        public AuthenticationController(AccountService accountService, UserService userService, OrganizationManagerService organizationManagerService, RequestManagerService requestManagerService)
        {
            _accountService = accountService;
            _userService = userService;
            _organizationManagerService = organizationManagerService;
            _requestManagerService = requestManagerService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            try
            {
                var (accessToken, refreshToken) = _accountService.Login(request.Account, request.Password);

                var response = new ResponseMessage();
                if (accessToken == null)
                {
                    response.Message = "Incorrect username, email or password!";
                    return BadRequest(response);
                }

                response.Message = "Login successfully!";
                response.Data = new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }


        [HttpPost("refresh-token")]
        public IActionResult CreateRefreshToken(RefreshTokenRequest request)
        {
            var response = new ResponseMessage();
            var (accessToken, refresherToken) = _accountService.ValidateRefreshToken(request.RefreshToken);
            if (accessToken == null)
            {
                return BadRequest("Refresh Token Failed!");
            }
            response.Message = "Refresh Token Successfully!";
            response.Data = new
            {
                AccessToken = accessToken,
                RefreshToken = refresherToken
            };
            return Ok(response);
        }

        [DBTransaction]
        [HttpPost("register")]
        public IActionResult Register(CreateUserRequest request)
        {
            try
            {
                if (request.AccountType.Equals("user"))
                {
                    var user = _userService.CreateUser(request);
                    if (user == null) return BadRequest("Email already exist!");
                    var response = new ResponseMessage()
                    {
                        Message = "Register successfully!",
                    };
                    return Login(new LoginRequest() { Account = request.Email, Password = request.Password });
                }
                else if (request.AccountType.Equals("organizationManager"))
                {
                    var omCreateRequest = new CreateNewOrganizationManagerRequest
                    {
                        Email = request.Email,
                        Password = request.Password,
                        Avatar = request.Avatar,
                        BirthDay = request.BirthDay,
                        FacebookUrl = request.FacebookUrl,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Gender = request.Gender,
                        PhoneNumber = request.PhoneNumber,
                        TiktokUrl = request.TiktokUrl,
                        Username = request.Username,
                        YoutubeUrl = request.YoutubeUrl
                    };
                    var organizationManager = _organizationManagerService.CreateOrganizationManager(omCreateRequest);
                    if (request == null) return BadRequest("Email already exist!");
                    var response = new ResponseMessage()
                    {
                        Message = "Register successfully!",
                    };
                    return Login(new LoginRequest() { Account = request.Email, Password = request.Password });
                }
                return BadRequest(new ResponseMessage
                {
                    Message = $"Error: Account type not found!"
                });

            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [DBTransaction]
        [HttpPost("register/request-manager")]
        public IActionResult RegisterRequestManager(CreateNewRequestManagerRequest request)
        {
            try
            {
                var requestManager = _requestManagerService.CreateRequestManager(request);
                if (request == null) return BadRequest("Email already exist!");
                var response = new ResponseMessage()
                {
                    Message = "Register successfully!",
                };
                return Login(new LoginRequest() { Account = request.Email, Password = request.Password });
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }


        [DBTransaction]
        [HttpPost("forgot-password")]
        public IActionResult Forgot(string email)
        {
            try
            {
                var result = _userService.ChangePasswordInCaseForgot(email);
                if (result == null) return NotFound("Email not found!");
                var response = new ResponseMessage()
                {
                    Message = "Change successfully!",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [DBTransaction]
        [HttpPost("reset-password")]
        [Authorize(Roles = "User, Member, OrganizationManager, RequestManager")]

        public IActionResult Reset(ResetPasswordRequest request)
        {
            try
            {
                var result = _accountService.ResetPassword(request);
                if (result == true) return Ok(new ResponseMessage()
                {
                    Message = "Change successfully!",
                });
                var response = new ResponseMessage()
                {
                    Message = "Change fail! Please check email again!",
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }


    }
}
