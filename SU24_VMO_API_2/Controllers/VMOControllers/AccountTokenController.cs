using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/account-token")]
    [ApiController]
    public class AccountTokenController : ControllerBase
    {
        private readonly AccountTokenService _accountTokenService;
        private readonly PaginationService<AccountToken> _paginationService;


        public AccountTokenController(AccountTokenService accountTokenService, PaginationService<AccountToken> paginationService)
        {
            _accountTokenService = accountTokenService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllAccountTokens(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var accounts = _accountTokenService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(accounts!, pageSize, pageNo, orderBy, orderByProperty)
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
