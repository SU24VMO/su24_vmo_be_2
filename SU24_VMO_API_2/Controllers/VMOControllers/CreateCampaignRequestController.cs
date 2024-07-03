using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/create-campaign-request")]
    [ApiController]
    public class CreateCampaignRequestController : ControllerBase
    {
        private readonly CreateCampaignRequestService _service;
        private readonly PaginationService<CreateCampaignRequest> _paginationService;

        public CreateCampaignRequestController(CreateCampaignRequestService service, PaginationService<CreateCampaignRequest> paginationService)
        {
            _service = service;
            _paginationService = paginationService;
        }


        [EnableQuery]
        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "Moderator")]
        public IActionResult GetAllCreateCampaignRequests(int? pageSize, int? pageNo)
        {
            try
            {
                var list = _service.GetCreateCampaignRequests();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(list!, pageSize, pageNo)
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
        [Authorize(Roles = "OrganizationManager, Volunteer")]
        [Route("create-new")]

        public async Task<IActionResult> CreateCampaignRequestAsync(Guid accountId, [FromForm] CreateCampaignRequestRequest request)
        {
            try
            {
                var result = await _service.CreateCampaignRequestAsync(accountId, request);
                if (result != null)
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Create successfully!",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseMessage()
                    {
                        Message = $"Error: Add failed!"
                    };
                    return BadRequest(response);
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

        public IActionResult AcceptOrRejectCreateCampaignRequest(UpdateCreateCampaignRequest request)
        {
            try
            {
                if (_service.AcceptOrRejectCreateCampaignRequest(request) == true)
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
    }
}
