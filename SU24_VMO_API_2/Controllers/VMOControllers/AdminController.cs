using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly PaginationService<Admin> _paginationService;


        public AdminController(AdminService adminService, PaginationService<Admin> paginationService)
        {
            _adminService = adminService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "Admin")]

        public IActionResult GetAllAdmins(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var admins = _adminService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(admins!, pageSize, pageNo, orderBy, orderByProperty)
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
