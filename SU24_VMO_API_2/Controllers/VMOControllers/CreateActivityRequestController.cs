using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/create-activity-request")]
    [ApiController]
    public class CreateActivityRequestController : ControllerBase
    {
        private readonly CreateActivityRequestService _createActivityRequestService;

        public CreateActivityRequestController(CreateActivityRequestService createActivityRequestService)
        {
            _createActivityRequestService = createActivityRequestService;
        }

        [HttpGet]
        [Route("all")]

        public IActionResult GetAllCreateActivityRequests()
        {
            try
            {
                var createActivityRequests = _createActivityRequestService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = createActivityRequests
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

        public  IActionResult CreateActivityRequest(Guid accountId, CreateActivityRequestRequest request)
        {
            try
            {
                var createActivityRequestCreated = _createActivityRequestService.CreateActivityRequest(accountId, request);
                if (createActivityRequestCreated != null)
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Add successfully!"
                    };

                    return Ok(response);
                }
                else
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Add failed!"
                    };

                    return BadRequest(response);
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


        [HttpGet]
        [Route("{createActivityRequestId}")]

        public IActionResult GetCreateActivityRequestById(Guid createActivityRequestId)
        {
            try
            {
                var createActivityRequest = _createActivityRequestService.GetById(createActivityRequestId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = createActivityRequest
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
        [Route("checking")]

        public IActionResult UpdateCreateActivity(UpdateCreateActivityRequest request)
        {
            try
            {
                _createActivityRequestService.AcceptOrRejectCreateActivityRequest(request);

                var response = new ResponseMessage()
                {
                    Message = "Update successfully!"
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
