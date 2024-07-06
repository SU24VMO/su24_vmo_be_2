using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using Transaction = BusinessObject.Models.Transaction;


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


        [HttpGet]
        [Route("all")]
        public IActionResult GetAllTransactions(int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var transactions = _transactionService.GetAllTransactions();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(transactions!, pageSize, pageNo, orderBy, orderByProperty)
                };

                return Ok(response);
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"Database error: {dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {argEx.ParamName} cannot be null."
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }

        [HttpGet]
        [Route("history-transaction/account/{accountId}")]

        public IActionResult GetHistoryTransactionByAccountId(Guid accountId, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty)
        {
            try
            {
                var transactions = _transactionService.GetTransactionByAccountId(accountId);
                return Ok(new ResponseMessage
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(transactions!, pageSize, pageNo, orderBy, orderByProperty)
                });
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"Database error: {dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {argEx.ParamName} cannot be null."
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
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
                        Message = $"Check successfully! Transaction status: {paymentLinkInformation.status}, Payer name: {paymentLinkInformation.transactions.FirstOrDefault()!.counterAccountName}",
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
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"Database error: {dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {argEx.ParamName} cannot be null."
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
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
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"Database error: {dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {argEx.ParamName} cannot be null."
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
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
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"Database error: {dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {argEx.ParamName} cannot be null."
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }


        [HttpGet]
        [Route("test-transaction")]

        public async Task<IActionResult> TestTransactionPayOs(int orderId)
        {
            try
            {
                var data = await _transactionService.GetData(orderId);

                return Ok(new ResponseMessage
                {
                    Message = $"Check successfully!",
                    Data = data
                });
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                var response = new ResponseMessage();
                if (dbEx.InnerException != null)
                {
                    response.Message = $"Database error: {dbEx.InnerException.Message}";
                }
                else
                {
                    response.Message = "Database update error.";
                }
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                // Log the exception details here if necessary
                return NotFound(response);
            }
            catch (ArgumentNullException argEx)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {argEx.ParamName} cannot be null."
                };
                // Log the exception details here if necessary
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
                // Log the exception details here if necessary
                return StatusCode(500, response); // Internal Server Error
            }
        }


    }
}
