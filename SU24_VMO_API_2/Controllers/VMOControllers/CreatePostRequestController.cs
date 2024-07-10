using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/create-post-request")]
    [ApiController]
    public class CreatePostRequestController : ControllerBase
    {
        private readonly CreatePostRequestService _createPostRequestService;
        private readonly PaginationService<CreatePostRequest> _paginationService;

        public CreatePostRequestController(PaginationService<CreatePostRequest> paginationService, CreatePostRequestService createPostRequestService)
        {
            _paginationService = paginationService;
            _createPostRequestService = createPostRequestService;
        }

        [HttpGet]
        [Authorize(Roles = "Moderator, Volunteer, Member, OrganizationManager, Admin")]
        [Route("all")]

        public IActionResult GetAllCreatePostRequests(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var requests = _createPostRequestService.GetAll();
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(requests, pageSize, pageNo, orderBy, orderByProperty)
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
        [Authorize(Roles = "Moderator, Volunteer, Member, OrganizationManager, Admin")]
        [Route("all/filter/post-title")]

        public IActionResult GetAllCreatePostRequestsByPostName(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty, string? postTitle)
        {
            try
            {
                var requests = _createPostRequestService.GetAllByPostName(postTitle);
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(requests, pageSize, pageNo, orderBy, orderByProperty)
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

        public async Task<IActionResult> CreateCreatePostRequestAsync([FromForm] CreatePostRequestRequest request)
        {
            try
            {
                var requestCreated = await _createPostRequestService.CreateCreatePostRequest(request);
                var response = new ResponseMessage()
                {
                    Message = "Create successfully!",
                };
                return Ok(response);
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
            catch(UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
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

        public IActionResult AcceptOrRejectCreatePostRequest(UpdateCreatePostRequest request)
        {
            try
            {
                _createPostRequestService.AcceptOrRejectCreatePostRequest(request);
                var response = new ResponseMessage()
                {
                    Message = "Update successfully!",
                };
                return Ok(response);

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
