using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/moderator")]
    [ApiController]
    public class ModeratorController : ControllerBase
    {
        private readonly ModeratorService _service;
        private readonly AccountService _accountService;
        private readonly PaginationService<Moderator> _paginationService;



        public ModeratorController(ModeratorService service, AccountService accountService, PaginationService<Moderator> paginationService)
        {
            _service = service;
            _accountService = accountService;
            _paginationService = paginationService;
        }


        [HttpGet]
        [Route("all")]

        public IActionResult GetAllModerators(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var moderators = _service.GetAllModerators();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(moderators!, pageSize, pageNo, orderBy, orderByProperty)
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
