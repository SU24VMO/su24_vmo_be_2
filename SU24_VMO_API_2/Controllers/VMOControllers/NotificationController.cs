using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly PaginationService<Notification> _paginationService;

        public NotificationController(PaginationService<Notification> paginationService, NotificationService notificationService)
        {

            _paginationService = paginationService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize(Roles = "Moderator, Volunteer, Member, OrganizationManager, Admin")]
        [Route("all/account/{accountId}")]

        public IActionResult GetAllNotificationsByAccountId(int? pageSize, int? pageNo, Guid accountId, string? orderBy, string? orderByProperty)
        {
            try
            {
                var notifications = _notificationService.GetAllNotificationsByAccountId(accountId);
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(notifications, pageSize, pageNo, orderBy, orderByProperty)
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


        [HttpPut]
        [Authorize(Roles = "Moderator, Volunteer, Member, OrganizationManager, Admin")]
        [Route("checking")]

        public IActionResult UpdateIsSeenNotification(Guid accountId)
        {
            try
            {
                if (_notificationService.UpdateIsSeenNotification(accountId) == true)
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Update successfully!",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Update fail! Request not found!",
                    };
                    return NotFound(response);
                }

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
