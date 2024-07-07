﻿using BusinessObject.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API_2.DTOs.Response;
using System.Collections.Generic;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/campaign")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly CampaignService _campaignService;
        private readonly PaginationService<Campaign> _paginationService;
        private readonly PaginationService<CampaignResponse> _paginationService2;



        public CampaignController(CampaignService campaignService, PaginationService<Campaign> paginationService, PaginationService<CampaignResponse> paginationService2)
        {
            _campaignService = campaignService;
            _paginationService = paginationService;
            _paginationService2 = paginationService2;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllCampaigns(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaigns();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignByCampaignName(string campaignName, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsByCampaignName(campaignName);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
                var campaign = _campaignService.GetCampaignResponseByCampaignId(campaignId);

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
        [Route("all/filter/campaign-name")]
        public IActionResult GetCampaignByCampaignId(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty, string? campaignName)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignResponsesByCampaignName(campaignName);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignByCampaignTypeId(Guid campaignTypeId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsByCampaignTypeId(campaignTypeId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignResponsesByCampaignTypeIdWithStatus(Guid? campaignTypeId, int? pageSize, int? pageNo, string? status, string? campaignName, string? createBy, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignResponsesByCampaignTypeIdWithStatus(campaignTypeId, status, campaignName, createBy);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignsWithActiveStatus(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithActiveStatus();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignsWithUnActiveStatus(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithUnActiveStatus();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignsWithDonatePhaseWasEnd(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithDonatePhaseWasEnd();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignsWithDonatePhaseIsProcessing(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithDonatePhaseIsProcessing();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignsWithProcessingPhaseIsProcessing(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithProcessingPhaseIsProcessing();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignsWithStatementPhaseIsProcessing(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithStatementPhaseIsProcessing();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignsWithEndStatus(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignsWithEndStatus();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignByCreateByOrganizationManagerId(Guid organizationManagerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerId(organizationManagerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignByCreateByOrganizationManagerIdWithOptionsPhaseInProcessingPhase(Guid organizationManagerId, int? pageSize, int? pageNo, string phase, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerIdWithOptionsPhaseInProcessingPhase(organizationManagerId, phase);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignByCreateByOrganizationManagerIdWithDonatePhaseIsProcessing(Guid organizationManagerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerIdWithDonatePhaseIsProcessing(organizationManagerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignByCreateByOrganizationManagerIdWithProcessingPhaseIsProcessing(Guid organizationManagerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerIdWithProcessingPhaseIsProcessing(organizationManagerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignByCreateByOrganizationManagerIdWithStatementIsProcessing(Guid organizationManagerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByOrganizationManagerIdWithStatementIsProcessing(organizationManagerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        public IActionResult GetAllCampaignByCreateByVolunteerId(Guid memberId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByVolunteerId(memberId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("create-by/volunteer/{memberId}/{phase}/processing-status")]
        public IActionResult GetAllCampaignByCreateByVolunteerIdWithOptionsPhaseInProcessingPhase(Guid memberId, int? pageSize, int? pageNo, string? phase, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByVolunteerIdWithOptionsPhaseInProcessingPhase(memberId, phase);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("create-by/volunteer/{memberId}/donate-phase/processing-status")]
        public IActionResult GetAllCampaignByCreateByVolunteerIdWithDonatePhaseIsProcessing(Guid memberId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByVolunteerIdWithDonatePhaseIsProcessing(memberId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("create-by/volunteer/{memberId}/processing-phase/processing-status")]
        public IActionResult GetAllCampaignByCreateByVolunteerIdWithProcessingPhaseIsProcessing(Guid memberId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByVolunteerIdWithProcessingPhaseIsProcessing(memberId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("create-by/volunteer/{memberId}/statement-phase/processing-status")]
        public IActionResult GetAllCampaignByCreateByVolunteerIdWithStatementPhaseIsProcessing(Guid memberId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var campaigns = _campaignService.GetAllCampaignByCreateByVolunteerIdWithStatementPhaseIsProcessing(memberId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService2.PaginateList(campaigns!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Authorize(Roles = "Volunteer, OrganizationManager")]
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
