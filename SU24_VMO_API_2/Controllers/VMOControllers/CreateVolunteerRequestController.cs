﻿using BusinessObject.Models;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API_2.DTOs.Request;

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
        [Authorize(Roles = "Member, Moderator")]
        [Route("update/volunteer-request-information")]

        public IActionResult UpdateCreateVolunteerRequest(Guid createVolunteerRequestId, UpdateCreateVolunteerRequestRequest request)
        {
            try
            {
                _service.UpdateCreateVolunteerRequest(createVolunteerRequestId, request);
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
