using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/organization")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly OrganizationService _organizationService;
        private readonly PaginationService<Organization> _paginationService;

        public OrganizationController(OrganizationService organizationService, PaginationService<Organization> paginationService)
        {
            _organizationService = organizationService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllOrganizations(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var organizations = _organizationService.GetAllOrganizations();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(organizations!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("all/filter/{organizationName}")]
        public IActionResult GetAllOrganizationByOrganizationName(string organizationName, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var organizations = _organizationService.GetAllOrganizationsByOrganizationName(organizationName);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(organizations!, pageSize, pageNo, orderBy, orderByProperty)
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
        [Route("all/filter/organization-manager/{organizationManagerId}")]
        public IActionResult GetAllOrganizationByOrganizationManagerId(Guid organizationManagerId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var organizations = _organizationService.GetAllOrganizationsByOrganizationManagerId(organizationManagerId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(organizations!, pageSize, pageNo, orderBy, orderByProperty)
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
