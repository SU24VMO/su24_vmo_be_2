using BusinessObject.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API_2.DTOs.Response;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/activity")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityService _activityService;
        private readonly PaginationService<Activity> _paginationService;
        private readonly PaginationService<ActivityResponse> _paginationService2;


        public ActivityController(ActivityService activityService, PaginationService<Activity> paginationService, PaginationService<ActivityResponse> paginationService2)
        {
            _activityService = activityService;
            _paginationService = paginationService;
            _paginationService2 = paginationService2;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "OrganizationManager, Member, Volunteer, Moderator")]

        public IActionResult GetAllActivitys(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var activities = _activityService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(activities, pageSize, pageNo, orderBy, orderByProperty)
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


        [HttpGet]
        [Route("create-by/organization-manager/{organizationManagerId}")]

        public IActionResult GetAllActivityWhichCreateByOM(Guid organizationManagerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var activities = _activityService.GetAllActivityWhichCreateByOM(organizationManagerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(activities!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("create-by/volunteer/{memberId}")]

        public IActionResult GetAllActivityWhichCreateByVolunteer(Guid memberId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var activities = _activityService.GetAllActivityWhichCreateByVolunteer(memberId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(activities!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Authorize(Roles = "Volunteer, OrganizationManager, Moderator")]

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
