using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/banking-account")]
    [ApiController]
    public class BankingAccountController : ControllerBase
    {
        private readonly BankingAccountService _bankingAccountService;

        public BankingAccountController(BankingAccountService bankingAccountService)
        {
            _bankingAccountService = bankingAccountService;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "OrganizationManager, Member, User")]

        public IActionResult GetAllBankingAccounts()
        {
            try
            {
                var bankingAccounts = _bankingAccountService.GetAll();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = bankingAccounts
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
        [Route("{id}")]
        public IActionResult GetBankingAccountById(Guid bankingAccountId)
        {
            try
            {
                var bankingAccount = _bankingAccountService.GetById(bankingAccountId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = bankingAccount
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
        [Route("create")]
        [Authorize(Roles = "Member, OrganizationManager, RequestManager")]

        public IActionResult CreateBankingAccount(CreateBankingAccountRequest request)
        {
            try
            {
                var bankingAccount = _bankingAccountService.CreateBankingAccount(request);
                if (bankingAccount == null)
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Create fail!",
                    };
                    return BadRequest(response);
                }
                else
                {
                    var response = new ResponseMessage()
                    {
                        Message = "Create successfully!",
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


        [HttpPut]
        [Route("update")]
        [Authorize(Roles = "Member, OrganizationManager, RequestManager")]

        public IActionResult UpdateBankingAccount(UpdateBankingAccountRequest request)
        {
            try
            {
                _bankingAccountService.UpdateBankingAccount(request);
                var response = new ResponseMessage()
                {
                    Message = "Update successfully!",
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
