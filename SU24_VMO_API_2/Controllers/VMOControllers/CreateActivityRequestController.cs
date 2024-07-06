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
        private readonly PaginationService<CreateActivityRequest> _paginationService;


        public CreateActivityRequestController(CreateActivityRequestService createActivityRequestService, PaginationService<CreateActivityRequest> paginationService)
        {
            _createActivityRequestService = createActivityRequestService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]

        public IActionResult GetAllCreateActivityRequests(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var createActivityRequests = _createActivityRequestService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(createActivityRequests, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("all/filter/activity-name/{activityName}")]

        public IActionResult GetAllCreateActivityRequestsByActivityTitle(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty, string activityName)
        {
            try
            {
                var createActivityRequests = _createActivityRequestService.GetAllByActivityTitle(activityName);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(createActivityRequests, pageSize, pageNo, orderBy, orderByProperty)
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

        public async Task<IActionResult> CreateActivityRequestAsync(Guid accountId,[FromForm] CreateActivityRequestRequest request)
        {
            try
            {
                var createActivityRequestCreated = await _createActivityRequestService.CreateActivityRequestAsync(accountId, request);
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
