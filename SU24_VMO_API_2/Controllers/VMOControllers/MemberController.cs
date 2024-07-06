using BusinessObject.Enums;
using BusinessObject.Models;
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
        private readonly PaginationService<Member> _paginationService;



        public MemberController(MemberService service, AccountService accountService, PaginationService<Member> paginationService)
        {
            _service = service;
            _accountService = accountService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]

        public IActionResult GetAllMembers(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var members = _service.GetAllMembers();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(members!, pageSize, pageNo, orderBy, orderByProperty)
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

        [HttpGet]
        [Route("all/filter/member-name/{memberName}")]

        public IActionResult GetAllMembersByMemberName(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty, string memberName)
        {
            try
            {
                var members = _service.GetMembersByMemberName(memberName);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(members!, pageSize, pageNo, orderBy, orderByProperty)
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
