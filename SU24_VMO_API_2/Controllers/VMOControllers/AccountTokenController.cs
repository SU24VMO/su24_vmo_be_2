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

        public AccountTokenController(AccountTokenService accountTokenService)
        {
            _accountTokenService = accountTokenService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllAccountTokens()
        {
            try
            {
                var accounts = _accountTokenService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = accounts
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
