﻿using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/organization-manager")]
    [ApiController]
    public class OrganizationManagerController : ControllerBase
    {

        private readonly OrganizationManagerService _service;
        private readonly AccountService _accountService;
        private readonly PaginationService<OrganizationManager> _paginationService;



        public OrganizationManagerController(OrganizationManagerService service, AccountService accountService, PaginationService<OrganizationManager> paginationService)
        {
            _service = service;
            _accountService = accountService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]

        public IActionResult GetAllOrganizationManagers(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var organizationManagers = _service.GetAllOrganizationManagers();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(organizationManagers!, pageSize, pageNo, orderBy, orderByProperty)
                };

                return Ok(response);
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
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
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }


        [HttpGet]
        [Route("all/filter/organization-manager-name/{organizationManagerName}")]

        public IActionResult GetAllOrganizationManagersByOrganizationManagerName(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty, string organizationManagerName)
        {
            try
            {
                var organizationManagers = _service.GetAllOrganizationManagersByOrganizationManagerName(organizationManagerName);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(organizationManagers!, pageSize, pageNo, orderBy, orderByProperty)
                };

                return Ok(response);
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"{dbEx.InnerException.Message}";
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
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (BadRequestException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{argEx.ParamName}"
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException unauEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{unauEx.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(403, response); // Internal Server Error
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"{ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }
    }
}
