using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/activity-image")]
    [ApiController]
    public class ActivityImageController : ControllerBase
    {

        private readonly ActivityImageService _activityImageService;

        public ActivityImageController(ActivityImageService activityImageService)
        {
            _activityImageService = activityImageService;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "OrganizationManager, Member, Volunteer")]

        public IActionResult GetAllActivityImages()
        {
            try
            {
                var activityImages = _activityImageService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = activityImages
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
        [Route("create-new")]
        [Authorize(Roles = "Volunteer, OrganizationManager, Moderator")]

        public IActionResult CreateActivityImage(CreateActivityImageRequest request)
        {
            try
            {
                var activityImage = _activityImageService.CreateActivityImage(request);
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
                        Message = "Create successfully!",
                    };

                    return Ok(response);
                }
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
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }
    }
}
