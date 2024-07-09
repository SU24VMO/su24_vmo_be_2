using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers
{
    [Route("api/testauth")]
    [ApiController]
    public class TestAuthenticationController : ControllerBase
    {
        private readonly AccountService _accountService;

        public TestAuthenticationController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("file")]
        public async Task<IActionResult> IndexAsync(IFormFile id)
        {


            FirebaseService s = new FirebaseService();
            //string filename = await s.UploadImage(imageFile);
            var content = await s.DownloadFiletest("dd253328-f18d-482b-9e07-9c1cb00e15e4.jpg");
            //byte[] file = await s.DownloadFile(filename);
            //byte[] file2 = FileSupporter.GenFileBytes(file);
            //string name = await s.UploadByByte(file2,"nam.docx");
            return File(content, "application/octet-stream", "dd253328-f18d-482b-9e07-9c1cb00e15e4.jpg");

            // return Ok("false");
        }


        [HttpPost("refresh-token")]
        public IActionResult CreateRefreshToken(RefreshTokenRequest request)
        {
            var response = new ResponseMessage();
            var principle = _accountService.GetClaimsPrincipal(request.RefreshToken);
            if (principle == null)
            {
                return BadRequest("Get Failed!");
            }
            response.Message = "Get successfully!";
            response.Data = principle;
            return Ok(response);
        }
    }
}
