using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/statement-phase")]
    [ApiController]
    public class StatementPhaseController : ControllerBase
    {
        private readonly StatementPhaseService _statementPhaseService;

        public StatementPhaseController(StatementPhaseService statementPhaseService)
        {
            _statementPhaseService = statementPhaseService;
        }


    }
}
