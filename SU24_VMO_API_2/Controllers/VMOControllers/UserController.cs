using BusinessObject.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly AccountService _accountService;


        public UserController(UserService service, AccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }

        [HttpPut("update-information/{userId}")]
        [Authorize(Roles = "User, Member")]
        public IActionResult UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                _service.UpdateUser(userId, request);
                var response = new ResponseMessage()
                {
                    Message = "Update successfully!",
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
        
    }
}
