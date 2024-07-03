using BusinessObject.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/member")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly MemberService _service;
        private readonly AccountService _accountService;


        public MemberController(MemberService service, AccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }

        [HttpPut("update-information/{memberId}")]
        [Authorize(Roles = "Volunteer, Member")]
        public IActionResult UpdateMember(Guid memberId, [FromBody] UpdateMemberRequest request)
        {
            try
            {
                _service.UpdateMember(memberId, request);
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
