﻿using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/donate-phase")]
    [ApiController]
    public class DonatePhaseController : ControllerBase
    {
        private readonly DonatePhaseService _donatePhaseService;
        private readonly PaginationService<DonatePhase> _paginationService;


        public DonatePhaseController(DonatePhaseService donatePhaseService, PaginationService<DonatePhase> paginationService)
        {
            _donatePhaseService = donatePhaseService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllDonatePhases(int? pageSize, int? pageNo)
        {
            try
            {
                var donatePhases = _donatePhaseService.GetAllDonatePhases();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(donatePhases!, pageSize, pageNo)
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
        [Authorize(Roles = "OrganizationManager, Member")]
        [Route("create-new")]

        public IActionResult CreateNewDonatePhase(DonatePhase donatePhase)
        {
            try
            {
                var donatePhaseCreated = _donatePhaseService.CreateDonatePhase(donatePhase);
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

        [HttpPut("status/update")]
        [Authorize(Roles = "Member, OrganizationManager, RequestManager")]
        public IActionResult UpdateDonatePhaseStatus(UpdateDonatePhaseStatusRequest request)
        {
            try
            {
                _donatePhaseService.UpdateDonatePhaseStatus(request);
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
                switch (ex)
                {
                    case NotFoundException:
                        return NotFound(response);
                    default:
                        return BadRequest(response);
                }
            }
        }
    }
}