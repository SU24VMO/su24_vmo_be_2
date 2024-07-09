using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API_2.DTOs.Request;
using SU24_VMO_API_2.DTOs.Response;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/processing-phase")]
    [ApiController]
    public class ProcessingPhaseController : ControllerBase
    {
        private readonly ProcessingPhaseService _processingPhaseService;
        private readonly PaginationService<ProcessingPhase> _paginationService;
        private readonly PaginationService<ProcessingPhaseResponse> _paginationService2;



        public ProcessingPhaseController(ProcessingPhaseService processingPhaseService, PaginationService<ProcessingPhase> paginationService,
            PaginationService<ProcessingPhaseResponse> paginationService2)
        {
            _processingPhaseService = processingPhaseService;
            _paginationService = paginationService;
            _paginationService2 = paginationService2;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllProcessingPhases(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var processingPhases = _processingPhaseService.GetAllProcessingPhases();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(processingPhases!, pageSize, pageNo, orderBy, orderByProperty)
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


        [HttpGet]
        [Route("all/filter/campaign-name")]
        public IActionResult GetAllProcessingPhasesWithCampaignName(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty, string? campaignName)
        {
            try
            {
                var processingPhases = _processingPhaseService.GetAllProcessingPhasesWithCampaignName(campaignName);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(processingPhases!, pageSize, pageNo, orderBy, orderByProperty)
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


        [HttpGet]
        [Route("create-by/organization-manager/{organizationManagerId}")]
        public IActionResult GetProcessingPhaseByOrganizationManagerId(Guid organizationManagerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var processingPhases = _processingPhaseService.GetProcessingPhaseByOMId(organizationManagerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(processingPhases!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("create-by/volunteer/{volunteerId}")]
        public IActionResult GetProcessingPhaseByMemberId(Guid volunteerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var processingPhases = _processingPhaseService.GetProcessingPhaseByMemberId(volunteerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(processingPhases!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("create-by/volunteer")]
        public IActionResult GetProcessingPhaseResponseByMemberId(Guid volunteerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var processingPhases = _processingPhaseService.GetProcessingPhaseResponseByMemberId(volunteerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(processingPhases!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("create-by/organization-manager")]
        public IActionResult GetProcessingPhaseResponseByOmId(Guid organizationManagerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var processingPhases = _processingPhaseService.GetProcessingPhaseResponseByOMId(organizationManagerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(processingPhases!, pageSize, pageNo, orderBy, orderByProperty)
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




        [HttpPut("update")]
        [Authorize(Roles = "Volunteer, OrganizationManager, Moderator")]
        public IActionResult UpdateProcessingPhase(UpdateProcessingPhaseRequest request)
        {
            try
            {
                _processingPhaseService.Update(request);
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


        [HttpPut("status/update")]
        [Authorize(Roles = "Volunteer, OrganizationManager, Moderator")]
        public IActionResult UpdateProcessingPhaseStatus(UpdateProcessingPhaseStatusRequest request)
        {
            try
            {
                _processingPhaseService.UpdateProcessingPhaseStatus(request);
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
