using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/statement-file")]
    [ApiController]
    public class StatementFileController : ControllerBase
    {
        private readonly StatementFileService _statementFileService;

        public StatementFileController(StatementFileService statementFileService)
        {
            _statementFileService = statementFileService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllStatementFile()
        {
            try
            {
                var statementFiles = _statementFileService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = statementFiles
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
        [Route("upload")]
        public async Task<IActionResult> UploadStatementFileAsync(CreateStatementFileRequest request)
        {
            try
            {
                var statementFile = await _statementFileService.UploadStatementFileAsync(request);
                if (statementFile == null)
                {
                    var response = new ResponseMessage()
                    {
                        Message = "StatementPhase not found!"
                    };
                    return NotFound(response);
                }
                else
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Upload successfully!"
                    };
                    return Ok(response);
                }
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
