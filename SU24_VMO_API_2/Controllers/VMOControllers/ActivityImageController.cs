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
    [Route("api/activity-image")]
    [ApiController]
    public class ActivityImageController : ControllerBase
    {

        private readonly ActivityImageService _activityImageService;
        private readonly PaginationService<ActivityImage> _paginationService;


        public ActivityImageController(ActivityImageService activityImageService, PaginationService<ActivityImage> paginationService)
        {
            _activityImageService = activityImageService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "OrganizationManager, Member, Volunteer")]

        public IActionResult GetAllActivityImages(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var activityImages = _activityImageService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(activityImages!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetActivityImageById(Guid activityImageId)
        {
            try
            {
                var activityImage = _activityImageService.GetById(activityImageId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = activityImage
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
        [Authorize(Roles = "Volunteer, OrganizationManager, Moderator")]

        public async Task<IActionResult> CreateActivityImage([FromForm] CreateActivityImageRequest request)
        {
            try
            {
                var activityImage = await _activityImageService.CreateActivityImage(request);
                if (activityImage == null)
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Create fail!",
                    };
                    return BadRequest(response);
                }
                else
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Đã đăng hình thành công!",
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


        [HttpPut]
        [Route("update")]
        [Authorize(Roles = "Volunteer, OrganizationManager, Moderator")]

        public IActionResult UpdateActivityImage(UpdateActivityImageRequest request)
        {
            try
            {
                _activityImageService.UpdateActivityImage(request);
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
