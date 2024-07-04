using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/moderator")]
    [ApiController]
    public class ModeratorController : ControllerBase
    {
        private readonly ModeratorService _service;
        private readonly AccountService _accountService;
        private readonly PaginationService<Moderator> _paginationService;



        public ModeratorController(ModeratorService service, AccountService accountService, PaginationService<Moderator> paginationService)
        {
            _service = service;
            _accountService = accountService;
            _paginationService = paginationService;
        }


        [HttpGet]
        [Route("all")]

        public IActionResult GetAllModerators(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var moderators = _service.GetAllModerators();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(moderators!, pageSize, pageNo, orderBy, orderByProperty)
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
