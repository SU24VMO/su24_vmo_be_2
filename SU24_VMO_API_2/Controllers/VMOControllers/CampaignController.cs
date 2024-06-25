using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using System.Collections.Generic;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/campaign")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly CampaignService _campaignService;
        private readonly PaginationService<Campaign> _paginationService;


        public CampaignController(CampaignService campaignService, PaginationService<Campaign> paginationService)
        {
            _campaignService = campaignService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllCampaigns(int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaigns();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo)
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
        [Route("all/filter/{campaignName}")]
        public IActionResult GetAllCampaignByCampaignName(string campaignName,int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsByCampaignName(campaignName);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo)
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
        [Route("{campaignId}")]
        public IActionResult GetCampaignById(Guid campaignId)
        {
            try
            {
                var campaign = _campaignService.GetCampaignByCampaignId(campaignId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = campaign
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
        public IActionResult GetAllCampaignByCreateByOrganizationManagerId(Guid organizationManagerId, int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerId(organizationManagerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo)
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
        [Route("create-by/member/{userId}")]
        public IActionResult GetAllCampaignByCreateByMemberId(Guid userId, int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByMemberId(userId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo)
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
        public async Task<IActionResult> CreateNewCampaign(CreateNewCampaignRequest request)
        {
            try
            {
                var campaignCreated = await _campaignService.CreateCampaign(request);

                var response = new ResponseMessage()
                {
                    Message = "Create successfully!"
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
        [Authorize(Roles = "Member, OrganizationManager")]
        public IActionResult UpdateCampaignRequest(Guid campaignId, UpdateCampaignRequest request)
        {
            try
            {
                _campaignService.UpdateCampaignRequest(campaignId, request);
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
