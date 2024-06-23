using BusinessObject.Models;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/create-member-request")]
    [ApiController]
    public class CreateMemberRequestController : ControllerBase
    {
        private readonly CreateMemberRequestService _service;
        private readonly PaginationService<CreateMemberRequest> _paginationService;

        public CreateMemberRequestController(CreateMemberRequestService service, PaginationService<CreateMemberRequest> paginationService)
        {
            _service = service;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Authorize(Roles = "RequestManager")]
        [Route("all")]

        public IActionResult GetAllCreateMemberRequests(int? pageSize, int? pageNo)
        {
            try
            {
                var requests = _service.GetAllCreateMemberRequests();
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(requests!, pageSize, pageNo)
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



        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("create-new")]

        public IActionResult Create(CreateMemberAccountRequest request)
        {
            try
            {
                var requestCreated = _service.CreateMemberRequest(request);
                if (requestCreated == null)
                {
                    var response = new ResponseMessage()
                    {
                        Message = "User not found!",
                    };
                    return BadRequest(response);
                }
                else
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Create successfully!",
                    };
                    return Ok(response);
                }
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

        [HttpPut]
        [Authorize(Roles = "RequestManager")]
        [Route("checking")]

        public IActionResult AcceptOrRejectCreateMemberRequest(UpdateCreateMemberAccountRequest request)
        {
            try
            {
                if (_service.AcceptOrRejectCreateMemberAccountRequest(request) == true)
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Update successfully!",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Update fail! Request not found!",
                    };
                    return NotFound(response);
                }

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
