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
    [Route("api/create-volunteer-request")]
    [ApiController]
    public class CreateVolunteerRequestController : ControllerBase
    {
        private readonly CreateVolunteerRequestService _service;
        private readonly PaginationService<CreateVolunteerRequest> _paginationService;

        public CreateVolunteerRequestController(CreateVolunteerRequestService service, PaginationService<CreateVolunteerRequest> paginationService)
        {
            _service = service;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Authorize(Roles = "Moderator")]
        [Route("all")]

        public IActionResult GetAllCreateVolunteerRequests(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var requests = _service.GetAllCreateVolunteerRequests();
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(requests!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Authorize(Roles = "Moderator")]
        [Route("all/filter/volunteer-name")]

        public IActionResult GetAllCreateVolunteerRequestsByVolunteerName(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty, string? volunteerName)
        {
            try
            {
                var requests = _service.GetAllCreateVolunteerRequestsByMemberName(volunteerName);
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(requests!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Authorize(Roles = "Member")]
        [Route("create-new")]

        public IActionResult CreateVolunteerRequest(CreateVolunteerAccountRequest request)
        {
            try
            {
                var requestCreated = _service.CreateVolunteerRequest(request);
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
        [Authorize(Roles = "Moderator")]
        [Route("checking")]

        public IActionResult AcceptOrRejectCreateVolunteerAccountRequest(UpdateCreateVolunteerAccountRequest request)
        {
            try
            {
                if (_service.AcceptOrRejectCreateVolunteerAccountRequest(request) == true)
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
