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
    [Route("api/achievement")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly AchievementService _achievementService;
        private readonly PaginationService<Achievement> _paginationService;


        public AchievementController(AchievementService achievementService, PaginationService<Achievement> paginationService)
        {
            _achievementService = achievementService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "OrganizationManager, Member, Volunteer")]

        public IActionResult GetAllAchievements(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var achievements = _achievementService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(achievements!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("{id}")]
        public IActionResult GetAchievementById(Guid achievementId)
        {
            try
            {
                var achievements = _achievementService.GetById(achievementId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = achievements
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


        [HttpPost]
        [Route("create-new")]
        [Authorize(Roles = "OrganizationManager")]

        public IActionResult CreateNewAchievement(CreateAchievementRequest request)
        {
            try
            {
                var achievements = _achievementService.CreateAchievement(request);

                var response = new ResponseMessage()
                {
                    Message = "Created successfully!",
                    Data = achievements
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

        [HttpPut("update")]
        [Authorize(Roles = "OrganizationManager")]
        public IActionResult UpdateAchievement(UpdateAchievementRequest request)
        {
            try
            {
                _achievementService.Update(request);
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
