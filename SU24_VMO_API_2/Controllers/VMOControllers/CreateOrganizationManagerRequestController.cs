using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/create-organization-manager-request")]
    [ApiController]
    public class CreateOrganizationManagerRequestController : ControllerBase
    {
        private readonly CreateOrganizationManagerRequestService _service;
        private readonly PaginationService<CreateOrganizationManagerRequest> _paginationService;


        public CreateOrganizationManagerRequestController(CreateOrganizationManagerRequestService service, PaginationService<CreateOrganizationManagerRequest> paginationService)
        {
            _service = service;
            _paginationService = paginationService;
        }
        [HttpGet]
        [Authorize(Roles = "Moderator, OrganizationManager, Admin")]
        [Route("all")]

        public IActionResult GetAllCreateOrganizationManagerRequests(int? pageSize, int? pageNo, string? orderBy)
        {
            try
            {
                var requests = _service.GetAllCreateOrganizationManagerRequests();
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(requests!, pageSize, pageNo, orderBy)
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
        [Authorize(Roles = "OrganizationManager")]
        [Route("create-new")]

        public IActionResult Create(CreateOrganizationManagerVerifiedRequest request)
        {
            try
            {
                var requestCreated = _service.CreateOrganizationManagerVerifiedRequest(request);
                if (requestCreated == null)
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Organization manager not found!",
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
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"Database error: {dbEx.InnerException.Message}";
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
                    Message = $"Error: {ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {argEx.ParamName} cannot be null."
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }

        [HttpPut]
        [Authorize(Roles = "Moderator")]
        [Route("checking")]

        public IActionResult AcceptOrRejectCreateOrganizationManagerRequest(UpdateCreateOrganizationManagerVerifiedAccountRequest request)
        {
            try
            {
                if (_service.AcceptOrRejectCreateOrganizationManagerAccountRequest(request) == true)
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
