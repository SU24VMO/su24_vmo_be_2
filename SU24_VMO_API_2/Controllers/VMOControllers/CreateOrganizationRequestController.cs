﻿using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API_2.DTOs.Request;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/create-organization-request")]
    [ApiController]
    public class CreateOrganizationRequestController : ControllerBase
    {
        private readonly CreateOrganizationRequestService _createOrganizationRequestService;
        private readonly PaginationService<CreateOrganizationRequest> _paginationService;

        public CreateOrganizationRequestController(CreateOrganizationRequestService createOrganizationRequestService, PaginationService<CreateOrganizationRequest> paginationService)
        {
            _createOrganizationRequestService = createOrganizationRequestService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Authorize(Roles = "Moderator, Volunteer, Member, OrganizationManager, Admin")]
        [Route("all")]

        public IActionResult GetAllCreateOrganizationRequests(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var requests = _createOrganizationRequestService.GetAll();
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(requests, pageSize, pageNo, orderBy, orderByProperty)
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
            catch (UnauthorizedAccessException unauEx)
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

        [HttpGet]
        [Authorize(Roles = "Moderator, Volunteer, Member, OrganizationManager, Admin")]
        [Route("all/filter/organization-name")]

        public IActionResult GetAllCreateOrganizationRequestsByOrganizationName(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty, string? organizationName)
        {
            try
            {
                var requests = _createOrganizationRequestService.GetAllByOrganizationName(organizationName);
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(requests, pageSize, pageNo, orderBy, orderByProperty)
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
            catch (UnauthorizedAccessException unauEx)
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


        [HttpPost]
        [Authorize(Roles = "OrganizationManager")]
        [Route("create-new")]

        public async Task<IActionResult> CreateOrganizationRequest(Guid organizationManagerId, [FromForm] CreateOrganizationRequestRequest request)
        {
            try
            {
                var requestCreated = await _createOrganizationRequestService.CreateOrganizationRequest(organizationManagerId, request);
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

        public IActionResult AcceptOrRejectCreateOrganizationRequest(AcceptOrRejectCreateOrganizationRequestRequest request)
        {
            try
            {
                if (_createOrganizationRequestService.AcceptOrRejectCreateOrganizationRequest(request) == true)
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
            catch (UnauthorizedAccessException unauEx)
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
        [Authorize(Roles = "OrganizationManager, Moderator")]
        [Route("update/organization-information")]

        public async Task<IActionResult> UpdateCreateOrganizationRequestRequest(Guid createOrganizationRequestRequestId, UpdateCreateOrganizationRequestRequest updateRequest)
        {
            try
            {
                _createOrganizationRequestService.UpdateCreateOrganizationRequestRequest(createOrganizationRequestRequestId, updateRequest);

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
            catch (UnauthorizedAccessException unauEx)
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
    }
}
