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
        public IActionResult GetAllCampaignByCampaignName(string campaignName, int? pageSize, int? pageNo)
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
        public IActionResult GetCampaignByCampaignId(Guid campaignId)
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
        [Route("all/filter/campaign-type")]
        public IActionResult GetAllCampaignByCampaignTypeId(Guid campaignTypeId, int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsByCampaignTypeId(campaignTypeId);

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
        [Route("all/filter/campaign-type/active-status")]
        public IActionResult GetAllCampaignWithActiveStatusByCampaignTypeId(Guid? campaignTypeId, int? pageSize, int? pageNo, string? status, string? campaignName)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsByCampaignTypeIdWithStatus(campaignTypeId, status, campaignName);

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
        [Route("all/filter/active-status")]
        public IActionResult GetAllCampaignsWithActiveStatus(int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithActiveStatus();

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
        [Route("all/filter/unactive-status")]
        public IActionResult GetAllCampaignsWithUnActiveStatus(int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithUnActiveStatus();

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
        [Route("all/filter/donate-phase/end-status")]
        public IActionResult GetAllCampaignsWithDonatePhaseWasEnd(int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithDonatePhaseWasEnd();

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
        [Route("all/filter/donate-phase/processing-status")]
        public IActionResult GetAllCampaignsWithDonatePhaseIsProcessing(int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithDonatePhaseIsProcessing();

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
        [Route("all/filter/processing-phase/processing-status")]
        public IActionResult GetAllCampaignsWithProcessingPhaseIsProcessing(int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithProcessingPhaseIsProcessing();

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
        [Route("all/filter/statement-phase/processing-status")]
        public IActionResult GetAllCampaignsWithStatementPhaseIsProcessing(int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithStatementPhaseIsProcessing();

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
        [Route("all/filter/end-status")]
        public IActionResult GetAllCampaignsWithEndStatus(int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithEndStatus();

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
        [Route("create-by/organization-manager/{organizationManagerId}/{phase}/processing-status")]
        public IActionResult GetAllCampaignByCreateByOrganizationManagerIdWithOptionsPhaseInProcessingPhase(Guid organizationManagerId, int? pageSize, int? pageNo, string? phase)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerIdWithOptionsPhaseInProcessingPhase(organizationManagerId, phase);

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
        [Route("create-by/organization-manager/{organizationManagerId}/donate-phase/processing-status")]
        public IActionResult GetAllCampaignByCreateByOrganizationManagerIdWithDonatePhaseIsProcessing(Guid organizationManagerId, int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerIdWithDonatePhaseIsProcessing(organizationManagerId);

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
        [Route("create-by/organization-manager/{organizationManagerId}/processing-phase/processing-status")]
        public IActionResult GetAllCampaignByCreateByOrganizationManagerIdWithProcessingPhaseIsProcessing(Guid organizationManagerId, int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerIdWithProcessingPhaseIsProcessing(organizationManagerId);

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
        [Route("create-by/organization-manager/{organizationManagerId}/statement-phase/processing-status")]
        public IActionResult GetAllCampaignByCreateByOrganizationManagerIdWithStatementIsProcessing(Guid organizationManagerId, int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerIdWithStatementIsProcessing(organizationManagerId);

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


        [HttpGet]
        [Route("create-by/member/{userId}/{phase}/processing-status")]
        public IActionResult GetAllCampaignByCreateByMemberIdWithOptionsPhaseInProcessingPhase(Guid userId, int? pageSize, int? pageNo, string? phase)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByMemberIdWithOptionsPhaseInProcessingPhase(userId, phase);

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
        [Route("create-by/member/{userId}/donate-phase/processing-status")]
        public IActionResult GetAllCampaignByCreateByMemberIdWithDonatePhaseIsProcessing(Guid userId, int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByMemberIdWithDonatePhaseIsProcessing(userId);

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
        [Route("create-by/member/{userId}/processing-phase/processing-status")]
        public IActionResult GetAllCampaignByCreateByMemberIdWithProcessingPhaseIsProcessing(Guid userId, int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByMemberIdWithProcessingPhaseIsProcessing(userId);

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
        [Route("create-by/member/{userId}/statement-phase/processing-status")]
        public IActionResult GetAllCampaignByCreateByMemberIdWithStatementPhaseIsProcessing(Guid userId, int? pageSize, int? pageNo)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByMemberIdWithStatementPhaseIsProcessing(userId);

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
