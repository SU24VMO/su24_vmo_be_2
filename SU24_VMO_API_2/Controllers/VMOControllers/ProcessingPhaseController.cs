using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/processing-phase")]
    [ApiController]
    public class ProcessingPhaseController : ControllerBase
    {
        private readonly ProcessingPhaseService _processingPhaseService;
        private readonly PaginationService<ProcessingPhase> _paginationService;


        public ProcessingPhaseController(ProcessingPhaseService processingPhaseService, PaginationService<ProcessingPhase> paginationService)
        {
            _processingPhaseService = processingPhaseService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllProcessingPhases(int? pageSize, int? pageNo)
        {
            try
            {
                var processingPhases = _processingPhaseService.GetAllProcessingPhases();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(processingPhases!, pageSize, pageNo)
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
        public IActionResult UpdateProcessingPhaseStatus(UpdateProcessingPhaseStatusRequest request)
        {
            try
            {
                _processingPhaseService.UpdateProcessingPhaseStatus(request);
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
