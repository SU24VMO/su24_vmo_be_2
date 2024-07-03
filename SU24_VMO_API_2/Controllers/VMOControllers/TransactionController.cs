using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {

        private readonly TransactionService _transactionService;
        private readonly PaginationService<Transaction> _paginationService;

        public TransactionController(TransactionService transactionService, PaginationService<Transaction> paginationService)
        {
            _transactionService = transactionService;
            _paginationService = paginationService;
        }

        [HttpPost]
        [Route("create-transaction")]
        public async Task<IActionResult> CreateTransaction(CreateTransactionRequest request)
        {
            try
            {
                var transactionResponse = await _transactionService.CreateTransactionAsync(request);

                if (transactionResponse != null)
                {
                    var response = new CreateTransactionResponse()
                    {
                        OrderID = transactionResponse.OrderID,
                        QRCode = transactionResponse.QRCode,
                    };
                    return Ok(response);

                }
                else
                {
                    return BadRequest("Can not create!");
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

        [HttpGet]
        [Route("history-transaction/account/{accountId}")]

        public IActionResult GetHistoryTransactionByAccountId(Guid accountId, int? pageSize, int? pageNo, string? orderBy)
        {
            try
            {
                var transactions = _transactionService.GetTransactionByAccountId(accountId);
                return Ok(new ResponseMessage
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(transactions!, pageSize, pageNo, orderBy)
                });
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
        [Route("check-transaction/send-email")]

        public async Task<IActionResult> CheckTransactionStatusAndSendEmail(CheckTransactionRequest request)
        {
            try
            {
                var paymentLinkInformation = await _transactionService.CheckAndSendEmailWithSuccessStatusAsync(request);
                if (paymentLinkInformation != null)
                {
                    if (paymentLinkInformation.status.Equals("PAID"))
                    {
                        return Ok(new ResponseMessage
                        {
                            Message = $"Check successfully! Transaction status: {paymentLinkInformation.status}, Email was sent successfully!",
                            Data = paymentLinkInformation.status
                        });
                    }
                    else
                    {
                        return Ok(new ResponseMessage
                        {
                            Message = $"Check successfully! Transaction status: {paymentLinkInformation.status}",
                            Data = paymentLinkInformation.status
                        });
                    }
                }
                else
                {
                    return NotFound(new ResponseMessage
                    {
                        Message = "Get fail. Transaction not found!"
                    });
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

        [HttpGet]
        [Route("check-transaction")]

        public async Task<IActionResult> CheckTransactionStatus(int orderId)
        {
            try
            {
                var paymentLinkInformation = await _transactionService.CheckTransactionAsync(orderId);
                if (paymentLinkInformation != null)
                {
                    return Ok(new ResponseMessage
                    {
                        Message = $"Check successfully! Transaction status: {paymentLinkInformation.status}",
                        Data = paymentLinkInformation.status
                    });
                }
                else
                {
                    return NotFound(new ResponseMessage
                    {
                        Message = "Get fail. Transaction not found!"
                    });
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
