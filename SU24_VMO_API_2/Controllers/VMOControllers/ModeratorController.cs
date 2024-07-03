using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/request-manager")]
    [ApiController]
    public class ModeratorController : ControllerBase
    {
        private readonly ModeratorService _service;
        private readonly AccountService _accountService;


        public ModeratorController(ModeratorService service, AccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }
    }
}
