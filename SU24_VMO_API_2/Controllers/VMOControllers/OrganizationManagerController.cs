using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/organization-manager")]
    [ApiController]
    public class OrganizationManagerController : ControllerBase
    {

        private readonly OrganizationManagerService _service;
        private readonly AccountService _accountService;
        private readonly PaginationService<OrganizationManager> _paginationService;



        public OrganizationManagerController(OrganizationManagerService service, AccountService accountService, PaginationService<OrganizationManager> paginationService)
        {
            _service = service;
            _accountService = accountService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]

        public IActionResult GetAllOrganizationManagers(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var organizationManagers = _service.GetAllOrganizationManagers();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(organizationManagers!, pageSize, pageNo, orderBy, orderByProperty)
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
