using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/activity")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityService _activityService;

        public ActivityController(ActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "OrganizationManager, Member, User")]

        public IActionResult GetAllActivitys()
        {
            try
            {
                var activities = _activityService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = activities
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
        public IActionResult GetActivityById(Guid activityId)
        {
            try
            {
                var activity = _activityService.GetById(activityId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = activity
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
        [Authorize(Roles = "Member, OrganizationManager, RequestManager")]

        public async Task<IActionResult> CreateActivityAsync([FromForm] CreateNewActivityRequest request)
        {
            try
            {
                var activity = await _activityService.CreateActivity(request);
                if (activity == null)
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
        [Authorize(Roles = "Member, OrganizationManager, RequestManager")]

        public IActionResult UpdateActivity(UpdateActivityRequest request)
        {
            try
            {
                _activityService.UpdateActivity(request);
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
