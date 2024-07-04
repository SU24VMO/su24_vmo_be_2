using BusinessObject.Enums;
using BusinessObject.Models;
using MailKit.Search;
using Net.payOS;
using Net.payOS.Types;
using NETCore.MailKit.Core;
using Org.BouncyCastle.Asn1.Ocsp;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Supporters.BankSupporter;
using SU24_VMO_API.Supporters.EmailSupporter;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using System.Text.RegularExpressions;
using Transaction = BusinessObject.Models.Transaction;

namespace SU24_VMO_API.Services
{
    public class TransactionService
    {
        PayOS payOs;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly DonatePhaseService _donatePhaseService;

        public TransactionService(ITransactionRepository transactionRepository, ICampaignRepository campaignRepository, IAccountRepository accountRepository,
            DonatePhaseService donatePhaseService)
        {
            payOs = new PayOS(PayOSConstants.ClientId, PayOSConstants.ApiKey, PayOSConstants.CheckSumKey);
            _transactionRepository = transactionRepository;
            _campaignRepository = campaignRepository;
            _accountRepository = accountRepository;
            _donatePhaseService = donatePhaseService;
        }


        public IEnumerable<Transaction>? GetAllTransactions()
        {
            var trans = _transactionRepository.GetAll();
            foreach (var transaction in trans)
            {
                if (transaction != null && transaction.Account != null) transaction.Account = null;
                if (transaction != null && transaction.BankingAccount != null) transaction.BankingAccount = null;
                if (transaction != null && transaction.Campaign != null && transaction.Campaign.Transactions != null) transaction.Campaign.Transactions = null;

            }
            return trans;
        }


        public async Task<CreateTransactionResponse?> CreateTransactionAsync(CreateTransactionRequest createTransactionRequest)
        {

            TryValidateRequest(createTransactionRequest);
            var item = new ItemData(createTransactionRequest.Note, 1, createTransactionRequest.Price);
            var items = new List<ItemData>
            {
                item
            };
            var transaction = new Transaction();
            var description = "Thanh toan chuyen khoan";


            if (createTransactionRequest.IsIncognito)
            {
                transaction = new Transaction
                {
                    TransactionID = Guid.NewGuid(),
                    CampaignID = createTransactionRequest.CampaignId,
                    TransactionType = TransactionType.Transafer,
                    Amount = createTransactionRequest.Price,
                    Note = createTransactionRequest.Note,
                    PayerName = "Người ủng hộ ẩn danh",
                    IsIncognito = true,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    AccountId = createTransactionRequest.AccountId,
                    TransactionStatus = TransactionStatus.Pending,
                    TransactionQRImageUrl = ""
                };
            }
            else
            {
                transaction = new Transaction
                {
                    TransactionID = Guid.NewGuid(),
                    CampaignID = createTransactionRequest.CampaignId,
                    TransactionType = TransactionType.Transafer,
                    Amount = createTransactionRequest.Price,
                    Note = createTransactionRequest.Note,
                    PayerName = "",
                    IsIncognito = false,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    AccountId = createTransactionRequest.AccountId,
                    TransactionStatus = TransactionStatus.Pending,
                    TransactionQRImageUrl = ""
                };
            }




            var response = new CreateTransactionResponse();
            var transactionCreated = _transactionRepository.Save(transaction);
            if (transactionCreated != null)
            {
                var orderId = transactionCreated.OrderId;
                PaymentData paymentData = new PaymentData(orderId, createTransactionRequest.Price, description, items, "https://localhost:7100/", "https://localhost:7100/");
                CreatePaymentResult createPayment = await payOs.createPaymentLink(paymentData);
                var linkCheckOut = createPayment.checkoutUrl;
                var bankSupport = new GetQRImage();
                var qrImage = await bankSupport.CreateQRCodeBaseOnUrlAsync(linkCheckOut);
                response.OrderID = orderId;
                response.QRCode = qrImage;
            }
            return response;
        }


        public async Task<PaymentLinkInformation?> CheckTransactionAsync(int orderId)
        {
            PaymentLinkInformation paymentLinkInfomation = await payOs.getPaymentLinkInformation(orderId);
            var transaction = _transactionRepository.GetTransactionByOrderId(orderId);
            if(transaction == null) { throw new NotFoundException("Giao dịch không tìm thấy!"); }
            if (paymentLinkInfomation != null && paymentLinkInfomation.status.Equals("PAID") && paymentLinkInfomation.transactions.FirstOrDefault() != null && !String.IsNullOrEmpty(paymentLinkInfomation.transactions.FirstOrDefault()!.counterAccountName))
            {
                transaction.PayerName = paymentLinkInfomation.transactions.FirstOrDefault()!.counterAccountName!;
                transaction.TransactionStatus = TransactionStatus.Success;
                _transactionRepository.Update(transaction);
            }
            return paymentLinkInfomation;
        }


        public async Task<PaymentLinkInformation?> CheckAndSendEmailWithSuccessStatusAsync(CheckTransactionRequest request)
        {
            PaymentLinkInformation paymentLinkInfomation = await payOs.getPaymentLinkInformation(request.OrderID);
            if (paymentLinkInfomation.status.Equals("PAID"))
            {
                var transaction = _transactionRepository.GetTransactionByOrderId(request.OrderID);
                if (transaction != null)
                {
                    transaction.PayerName = request.FirstName + " " + request.LastName;
                    transaction.TransactionStatus = TransactionStatus.Success;
                    _transactionRepository.Update(transaction);


                    var campaign = _campaignRepository.GetById(transaction.CampaignID);
                    EmailSupporter.SendEmailWithSuccessDonate(request.Email, request.FirstName + " " + request.LastName, campaign!.Name!, transaction.Amount, transaction.CreateDate);

                    _donatePhaseService.UpdateDonatePhaseByCampaignIdAndAmountDonate(campaign.CampaignID, transaction.Amount);


                    if (paymentLinkInfomation != null && paymentLinkInfomation.transactions.FirstOrDefault() != null && !String.IsNullOrEmpty(paymentLinkInfomation.transactions.FirstOrDefault()!.counterAccountName))
                    {
                        transaction.PayerName = paymentLinkInfomation.transactions.FirstOrDefault()!.counterAccountName!;
                        _transactionRepository.Update(transaction);
                    }
                }
            }
            return paymentLinkInfomation;
        }


        public IEnumerable<Transaction?> GetTransactionByAccountId(Guid accountId)
        {
            var transactions = _transactionRepository.GetHistoryTransactionByAccountId(accountId);
            return transactions;
        }



        private void TryValidateRequest(CreateTransactionRequest createTransactionRequest)
        {
            var campaign = _campaignRepository.GetById(createTransactionRequest.CampaignId);
            if (campaign == null)
            {
                throw new Exception("Campaign not found!");
            }

            var account = _accountRepository.GetById(createTransactionRequest.AccountId);
            if (account == null)
            {
                throw new Exception("Account not found!");
            }

            if (createTransactionRequest.Note == null)
            {
                throw new Exception("Note can not be null!");
            }
            if (createTransactionRequest.Price < 2000)
            {
                throw new Exception("Amount donate must be greater or equal than 2000 dong!");
            }

        }
    }
}
