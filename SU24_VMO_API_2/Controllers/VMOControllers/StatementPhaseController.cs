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

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/statement-phase")]
    [ApiController]
    public class StatementPhaseController : ControllerBase
    {
        private readonly StatementPhaseService _statementPhaseService;
        private readonly PaginationService<StatementPhase> _paginationService;

        public StatementPhaseController(StatementPhaseService statementPhaseService, PaginationService<StatementPhase> paginationService)
        {
            _statementPhaseService = statementPhaseService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllStatementPhase(int? pageSize, int? pageNo)
        {
            try
            {
                var statementFiles = _statementPhaseService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(statementFiles, pageSize, pageNo)
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


        [HttpPut("update")]
        [Authorize(Roles = "Member, OrganizationManager, RequestManager")]
        public IActionResult UpdateStatementPhase(UpdateStatementPhaseRequest request)
        {
            try
            {
                _statementPhaseService.UpdateStatementPhase(request);
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
        [Authorize(Roles = "Member, OrganizationManager, RequestManager")]
        public IActionResult UpdateStatementPhaseStatus(UpdateStatementPhaseStatusRequest request)
        {
            try
            {
                _statementPhaseService.UpdateStatementPhaseStatus(request);
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
