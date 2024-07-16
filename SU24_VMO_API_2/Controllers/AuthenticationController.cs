using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using Microsoft.AspNetCore.Authorization;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Attributes;
using SU24_VMO_API_2.DTOs.Request.AccountRequest;
using Microsoft.EntityFrameworkCore;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using BusinessObject.Models;

namespace SU24_VMO_API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly MemberService _memberService;
        private readonly OrganizationManagerService _organizationManagerService;
        private readonly ModeratorService _moderatorService;





        public AuthenticationController(AccountService accountService, MemberService memberService,
            OrganizationManagerService organizationManagerService, ModeratorService moderatorService)
        {
            _accountService = accountService;
            _memberService = memberService;
            _organizationManagerService = organizationManagerService;
            _moderatorService = moderatorService;
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
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
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
        public IActionResult Register(CreateMemberRequest request)
        {
            try
            {
                if (request.AccountType.Equals("member"))
                {
                    var member = _memberService.CreateMember(request);
                    if (member == null) return BadRequest("Email already exist!");
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
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }


        [HttpPost("register/send-otp")]
        public async Task<IActionResult> SendOTP(CreateMemberRequest request)
        {
            try
            {
                var result = await _memberService.SendOTPWhenCreateNewUser(request);
                var response = new ResponseMessage()
                {
                    Message = "Send successfully!",
                    Data = result
                };
                return Ok(response);

            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }

        [DBTransaction]
        [HttpPost("register/moderator")]
        public IActionResult RegisterModerator(CreateNewRequestManagerRequest request)
        {
            try
            {
                var moderator = _moderatorService.CreateModerator(request);
                if (request == null) return BadRequest("Email already exist!");
                var response = new ResponseMessage()
                {
                    Message = "Register successfully!",
                };
                return Login(new LoginRequest() { Account = request.Email, Password = request.Password });
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }


        [DBTransaction]
        [HttpPost("forgot-password")]
        public IActionResult Forgot(string email)
        {
            try
            {
                var result = _memberService.ChangePasswordInCaseForgot(email);
                if (result == null) return NotFound("Email not found!");
                var response = new ResponseMessage()
                {
                    Message = "Change successfully!",
                    Data = result
                };
                return Ok(response);
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }

        [DBTransaction]
        [HttpPost("reset-password")]
        [Authorize(Roles = "Volunteer, Member, OrganizationManager, Moderator")]

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
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }


        [DBTransaction]
        [HttpPost("forgot-password/reset-password")]

        public IActionResult ResetForgotPassword(ResetPasswordRequest request)
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
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }



        [HttpPost("check-password")]
        [Authorize(Roles = "Volunteer, Member, OrganizationManager, Moderator, Admin")]
        public IActionResult CheckingPassword(CheckPasswordRequest request)
        {
            try
            {
                var result = _accountService.CheckPassword(request);
                var response = new ResponseMessage()
                {
                    Message = "Check successfully!",
                    Data = result
                };
                return Ok(response);
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }

    }
}
