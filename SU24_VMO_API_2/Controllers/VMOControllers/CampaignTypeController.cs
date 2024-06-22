using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/campaign-type")]
    [ApiController]
    public class CampaignTypeController : ControllerBase
    {
        private readonly CampaignTypeService _campaignTypeService;

        public CampaignTypeController(CampaignTypeService campaignTypeService)
        {
            _campaignTypeService = campaignTypeService;
        }



        [HttpGet]
        [Route("all")]

        public IActionResult GetAllCampaignType() 
        {
            try
            {
                var campaignTypes = _campaignTypeService.GetCampaignTypes();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = campaignTypes
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
        [Route("{campaignTypeId}")]

        public IActionResult GetCampaignTypeById(Guid campaignTypeId)
        {
            try
            {
                var campaignType = _campaignTypeService.GetCampaignTypeById(campaignTypeId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = campaignType
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
        [Route("create")]

        //[Authorize(Roles = "RequestManager, Admin")]
        public IActionResult CreateCampaignType(CreateCampaignTypeRequest typeRequest)
        {
            try
            {
                var campaignCreated = _campaignTypeService.CreateCampaignType(typeRequest);

                var response = new ResponseMessage()
                {
                    Message = "Create successfully!",
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
        [Authorize(Roles = "Admin, RequestManager")]
        public IActionResult UpdateCampaignTypeRequest(UpdateCampaignTypeRequest request)
        {
            try
            {
                _campaignTypeService.UpdateCampaignType(request);
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
