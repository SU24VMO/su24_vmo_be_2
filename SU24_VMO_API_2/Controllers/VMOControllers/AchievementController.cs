using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/achievement")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly AchievementService _achievementService;

        public AchievementController(AchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "OrganizationManager, Member, Volunteer")]

        public IActionResult GetAllAchievements()
        {
            try
            {
                var achievements = _achievementService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = achievements
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
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
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
