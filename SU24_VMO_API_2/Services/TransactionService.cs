using BusinessObject.Enums;
using BusinessObject.Models;
using MailKit.Search;
using Microsoft.OData.UriParser;
using Net.payOS;
using Net.payOS.Types;
using Net.payOS.Utils;
using NETCore.MailKit.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using SU24_VMO_API_2.DTOs.Request;
using SU24_VMO_API_2.DTOs.Response.PayosReponse;
using SU24_VMO_API_2.DTOs.Response;
using System;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1.Cms;
using Transaction = BusinessObject.Models.Transaction;
using System.Globalization;
using Org.BouncyCastle.Asn1.X509;


namespace SU24_VMO_API.Services
{
    public class TransactionService
    {
        PayOS payOs;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBankingAccountRepository _bankingAccountRepository;
        private readonly IDonatePhaseRepository _donatePhaseRepository;
        private readonly FirebaseService _firebaseService;
        private readonly DonatePhaseService _donatePhaseService;
        private readonly ICreateCampaignRequestRepository _createCampaignRequestRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;

        public TransactionService(ITransactionRepository transactionRepository, ICampaignRepository campaignRepository, IAccountRepository accountRepository,
            DonatePhaseService donatePhaseService, IBankingAccountRepository bankingAccountRepository, FirebaseService firebaseService,
            ICreateCampaignRequestRepository createCampaignRequestRepository, IMemberRepository memberRepository,
            IOrganizationManagerRepository organizationManagerRepository, IDonatePhaseRepository donatePhaseRepository)
        {
            payOs = new PayOS(PayOSConstants.ClientId, PayOSConstants.ApiKey, PayOSConstants.CheckSumKey);
            _transactionRepository = transactionRepository;
            _campaignRepository = campaignRepository;
            _accountRepository = accountRepository;
            _donatePhaseService = donatePhaseService;
            _bankingAccountRepository = bankingAccountRepository;
            _firebaseService = firebaseService;
            _createCampaignRequestRepository = createCampaignRequestRepository;
            _memberRepository = memberRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _donatePhaseRepository = donatePhaseRepository;
        }


        public IEnumerable<Transaction> GetAllTransactions()
        {
            var trans = _transactionRepository.GetAll();
            foreach (var transaction in trans)
            {
                if (transaction != null && transaction.Account != null)
                {
                    transaction.Account.BankingAccounts = null;
                    transaction.Account.AccountTokens = null;
                    transaction.Account.Notifications = null;
                    transaction.Account.Transactions = null;
                }
                if (transaction != null && transaction.BankingAccount != null) transaction.BankingAccount = null;
                if (transaction != null && transaction.Campaign != null && transaction.Campaign.Transactions != null) transaction.Campaign.Transactions = null;

            }
            return trans;
        }


        public int CalculateNumberOfTransactionDonate()
        {
            var transactions = _transactionRepository.GetAll().Where(o => o.TransactionType == TransactionType.Receive && o.TransactionStatus == TransactionStatus.Success);
            int count = transactions.Count();
            return count;
        }


        public float CalculateTotalAmountOfTransactionByAdmin()
        {
            var transactionsTranfer = _transactionRepository.GetAll().Where(o => o.TransactionType == TransactionType.Transfer && o.TransactionStatus == TransactionStatus.Success);
            float totalTransfer = 0;
            foreach (var transaction in transactionsTranfer)
            {
                totalTransfer += transaction.Amount;
            }

            var transactionsReceive = _transactionRepository.GetAll().Where(o => o.TransactionType == TransactionType.Receive && o.TransactionStatus == TransactionStatus.Success);
            float totalReceive = 0;
            foreach (var transaction in transactionsReceive)
            {
                totalReceive += transaction.Amount;
            }

            return totalReceive - totalTransfer;
        }



        public async Task<IEnumerable<TransactionForStatementByAdmin>> GetTransactionReceiveForStatementByAdmin(string? campaignName)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var transactions = _transactionRepository.GetAll().Where(o => o.TransactionType == TransactionType.Receive && o.TransactionStatus == TransactionStatus.Success);
                var listResponse = new List<TransactionForStatementByAdmin>();
                foreach (var transaction in transactions)
                {
                    var campaign = _campaignRepository.GetById(transaction.CampaignID)!;
                    var createCampaignRequest =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;

                    PaymentLinkInformation paymentLinkInformation = await payOs.getPaymentLinkInformation(transaction.OrderId);


                    if (createCampaignRequest.CreateByMember != null)
                    {
                        var receiveAccount = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember)!;
                        var usernameReceiveAccount = _accountRepository.GetById(receiveAccount.AccountID)!.Username;
                        var sendAccount = _accountRepository.GetById(transaction.AccountId)!;
                        listResponse.Add(new TransactionForStatementByAdmin
                        {
                            Amount = transaction.Amount,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            IsCognito = transaction.IsIncognito,
                            CampaignName = campaign.Name,
                            Status = transaction.TransactionStatus,
                            Date = transaction.CreateDate.ToString("dd/MM/yyyy"),
                            Time = transaction.CreateDate.ToString("hh:mmtt", CultureInfo.InvariantCulture),
                            ReceiveAccount = usernameReceiveAccount,
                            SendAccount = sendAccount.Username,
                            Platform = paymentLinkInformation.transactions.FirstOrDefault()!.counterAccountName.ToLower().Contains("momo") ? "Ví điện tử" : "Ebanking"
                        });
                    }
                    if (createCampaignRequest.CreateByOM != null)
                    {
                        var receiveAccount = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM)!;
                        var usernameReceiveAccount = _accountRepository.GetById(receiveAccount.AccountID)!.Username;
                        var sendAccount = _accountRepository.GetById(transaction.AccountId)!;
                        listResponse.Add(new TransactionForStatementByAdmin
                        {
                            Amount = transaction.Amount,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            IsCognito = transaction.IsIncognito,
                            CampaignName = campaign.Name,
                            Status = transaction.TransactionStatus,
                            Date = transaction.CreateDate.ToString("dd/MM/yyyy"),
                            Time = transaction.CreateDate.ToString("hh:mmtt", CultureInfo.InvariantCulture),
                            ReceiveAccount = usernameReceiveAccount,
                            SendAccount = sendAccount.Username,
                            Platform = paymentLinkInformation.transactions.FirstOrDefault()!.counterAccountName.ToLower().Contains("momo") ? "Ví điện tử" : "Ebanking"
                        });
                    }
                }
                return listResponse.Where(x => x.CampaignName.ToLower().Contains(campaignName.ToLower().Trim()));
            }
            else
            {
                var transactions = _transactionRepository.GetAll().Where(o => o.TransactionType == TransactionType.Receive && o.TransactionStatus == TransactionStatus.Success);
                var listResponse = new List<TransactionForStatementByAdmin>();
                foreach (var transaction in transactions)
                {
                    var campaign = _campaignRepository.GetById(transaction.CampaignID)!;
                    var createCampaignRequest =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;

                    PaymentLinkInformation paymentLinkInformation = await payOs.getPaymentLinkInformation(transaction.OrderId);


                    if (createCampaignRequest.CreateByMember != null)
                    {
                        var receiveAccount = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember)!;
                        var usernameReceiveAccount = _accountRepository.GetById(receiveAccount.AccountID)!.Username;
                        var sendAccount = _accountRepository.GetById(transaction.AccountId)!;
                        listResponse.Add(new TransactionForStatementByAdmin
                        {
                            Amount = transaction.Amount,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            IsCognito = transaction.IsIncognito,
                            CampaignName = campaign.Name,
                            Status = transaction.TransactionStatus,
                            Date = transaction.CreateDate.ToString("dd/MM/yyyy"),
                            Time = transaction.CreateDate.ToString("hh:mmtt", CultureInfo.InvariantCulture),
                            ReceiveAccount = usernameReceiveAccount,
                            SendAccount = sendAccount.Username,
                            Platform = paymentLinkInformation.transactions.FirstOrDefault()!.counterAccountName.ToLower().Contains("momo") ? "Ví điện tử" : "Ebanking"
                        });
                    }
                    if (createCampaignRequest.CreateByOM != null)
                    {
                        var receiveAccount = _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM)!;
                        var usernameReceiveAccount = _accountRepository.GetById(receiveAccount.AccountID)!.Username;
                        var sendAccount = _accountRepository.GetById(transaction.AccountId)!;
                        listResponse.Add(new TransactionForStatementByAdmin
                        {
                            Amount = transaction.Amount,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            IsCognito = transaction.IsIncognito,
                            CampaignName = campaign.Name,
                            Status = transaction.TransactionStatus,
                            Date = transaction.CreateDate.ToString("dd/MM/yyyy"),
                            Time = transaction.CreateDate.ToString("hh:mmtt", CultureInfo.InvariantCulture),
                            ReceiveAccount = usernameReceiveAccount,
                            SendAccount = sendAccount.Username,
                            Platform = paymentLinkInformation.transactions.FirstOrDefault()!.counterAccountName.ToLower().Contains("momo") ? "Ví điện tử" : "Ebanking"
                        });
                    }
                }
                return listResponse;
            }
        }



        public async Task<IEnumerable<TransactionForStatementByAdmin>> GetTransactionSendForStatementByAdmin(string? campaignName)
        {
            if (!String.IsNullOrEmpty(campaignName))
            {
                var transactions = _transactionRepository.GetAll().Where(o =>
                    o.TransactionType == TransactionType.Transfer && o.TransactionStatus == TransactionStatus.Success);
                var listResponse = new List<TransactionForStatementByAdmin>();
                foreach (var transaction in transactions)
                {
                    var campaign = _campaignRepository.GetById(transaction.CampaignID)!;
                    var createCampaignRequest =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;

                    //PaymentLinkInformation paymentLinkInformation = await payOs.getPaymentLinkInformation(transaction.OrderId);


                    if (createCampaignRequest.CreateByMember != null)
                    {
                        var receiveAccount = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember)!;
                        var usernameReceiveAccount = _accountRepository.GetById(receiveAccount.AccountID)!.Username;
                        var sendAccount = _accountRepository.GetById(transaction.AccountId)!;
                        listResponse.Add(new TransactionForStatementByAdmin
                        {
                            Amount = transaction.Amount,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            IsCognito = transaction.IsIncognito,
                            CampaignName = campaign.Name,
                            Status = transaction.TransactionStatus,
                            Date = transaction.CreateDate.ToString("dd/MM/yyyy"),
                            Time = transaction.CreateDate.ToString("hh:mmtt", CultureInfo.InvariantCulture),
                            ReceiveAccount = usernameReceiveAccount,
                            SendAccount = sendAccount.Username,
                        });
                    }

                    if (createCampaignRequest.CreateByOM != null)
                    {
                        var receiveAccount =
                            _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM)!;
                        var usernameReceiveAccount = _accountRepository.GetById(receiveAccount.AccountID)!.Username;
                        var sendAccount = _accountRepository.GetById(transaction.AccountId)!;
                        listResponse.Add(new TransactionForStatementByAdmin
                        {
                            Amount = transaction.Amount,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            IsCognito = transaction.IsIncognito,
                            CampaignName = campaign.Name,
                            Status = transaction.TransactionStatus,
                            Date = transaction.CreateDate.ToString("dd/MM/yyyy"),
                            Time = transaction.CreateDate.ToString("hh:mmtt", CultureInfo.InvariantCulture),
                            ReceiveAccount = usernameReceiveAccount,
                            SendAccount = sendAccount.Username,
                        });
                    }
                }

                return listResponse.Where(x => x.CampaignName.ToLower().Contains(campaignName.ToLower().Trim()));
            }
            else
            {
                var transactions = _transactionRepository.GetAll().Where(o =>
                    o.TransactionType == TransactionType.Transfer && o.TransactionStatus == TransactionStatus.Success);
                var listResponse = new List<TransactionForStatementByAdmin>();
                foreach (var transaction in transactions)
                {
                    var campaign = _campaignRepository.GetById(transaction.CampaignID)!;
                    var createCampaignRequest =
                        _createCampaignRequestRepository.GetCreateCampaignRequestByCampaignId(campaign.CampaignID)!;

                    //PaymentLinkInformation paymentLinkInformation = await payOs.getPaymentLinkInformation(transaction.OrderId);


                    if (createCampaignRequest.CreateByMember != null)
                    {
                        var receiveAccount = _memberRepository.GetById((Guid)createCampaignRequest.CreateByMember)!;
                        var usernameReceiveAccount = _accountRepository.GetById(receiveAccount.AccountID)!.Username;
                        var sendAccount = _accountRepository.GetById(transaction.AccountId)!;
                        listResponse.Add(new TransactionForStatementByAdmin
                        {
                            Amount = transaction.Amount,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            IsCognito = transaction.IsIncognito,
                            CampaignName = campaign.Name,
                            Status = transaction.TransactionStatus,
                            Date = transaction.CreateDate.ToString("dd/MM/yyyy"),
                            Time = transaction.CreateDate.ToString("hh:mmtt", CultureInfo.InvariantCulture),
                            ReceiveAccount = usernameReceiveAccount,
                            SendAccount = sendAccount.Username,
                        });
                    }

                    if (createCampaignRequest.CreateByOM != null)
                    {
                        var receiveAccount =
                            _organizationManagerRepository.GetById((Guid)createCampaignRequest.CreateByOM)!;
                        var usernameReceiveAccount = _accountRepository.GetById(receiveAccount.AccountID)!.Username;
                        var sendAccount = _accountRepository.GetById(transaction.AccountId)!;
                        listResponse.Add(new TransactionForStatementByAdmin
                        {
                            Amount = transaction.Amount,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            IsCognito = transaction.IsIncognito,
                            CampaignName = campaign.Name,
                            Status = transaction.TransactionStatus,
                            Date = transaction.CreateDate.ToString("dd/MM/yyyy"),
                            Time = transaction.CreateDate.ToString("hh:mmtt", CultureInfo.InvariantCulture),
                            ReceiveAccount = usernameReceiveAccount,
                            SendAccount = sendAccount.Username,
                        });
                    }
                }

                return listResponse;
            }
        }



        public IEnumerable<TransactionResponse> GetAllNumberRecentlyTransactions(int? numberOfTransaction)
        {
            if (numberOfTransaction != null)
            {
                var trans = GetAllTransactions().Where(t => t.TransactionStatus == TransactionStatus.Success)
                .OrderByDescending(transaction => transaction.CreateDate) // Order by CreateDate descending
                .Take((int)numberOfTransaction) // Take the top 6 transactions
                .ToList(); // Convert to a list if needed;
                var transReponse = new List<TransactionResponse>();
                if (trans != null)
                {
                    foreach (var tran in trans)
                    {
                        transReponse.Add(new TransactionResponse
                        {
                            AccountId = tran.AccountId,
                            Amount = tran.Amount,
                            Avatar = tran.Account != null ? tran.Account.Avatar : "Không có ảnh đại diện",
                            BankingAccountID = tran.BankingAccountID,
                            CampaignID = tran.CampaignID,
                            CreateDate = tran.CreateDate,
                            IsIncognito = tran.IsIncognito,
                            OrderId = tran.OrderId,
                            Note = tran.Note,
                            TransactionID = tran.TransactionID,
                            PayerName = tran.IsIncognito ? "Người ủng hộ ẩn danh" : tran.PayerName,
                            TransactionImageUrl = tran.TransactionImageUrl,
                            TransactionType = tran.TransactionType,
                            TransactionStatus = tran.TransactionStatus,
                            DonatationPeriod = CalculateDonationPeriod(tran.CreateDate),
                            DonateStatus = "Vừa ủng hộ"
                        });
                    }
                }
                return transReponse.Where(t => t.TransactionType == TransactionType.Receive);
            }
            else
            {
                var trans = GetAllTransactions().Where(t => t.TransactionStatus == TransactionStatus.Success)
                .OrderByDescending(transaction => transaction.CreateDate) // Order by CreateDate descending
                .Take(6) // Take the top 6 transactions
                .ToList(); // Convert to a list if needed;
                var transReponse = new List<TransactionResponse>();
                if (trans != null)
                {
                    foreach (var tran in trans)
                    {
                        transReponse.Add(new TransactionResponse
                        {
                            AccountId = tran.AccountId,
                            Amount = tran.Amount,
                            Avatar = tran.Account != null ? tran.Account.Avatar : "Không có ảnh đại diện",
                            BankingAccountID = tran.BankingAccountID,
                            CampaignID = tran.CampaignID,
                            CreateDate = tran.CreateDate,
                            IsIncognito = tran.IsIncognito,
                            OrderId = tran.OrderId,
                            Note = tran.Note,
                            TransactionID = tran.TransactionID,
                            PayerName = tran.IsIncognito ? "Người ủng hộ ẩn danh" : tran.PayerName,
                            TransactionImageUrl = tran.TransactionImageUrl,
                            TransactionType = tran.TransactionType,
                            TransactionStatus = tran.TransactionStatus,
                            DonatationPeriod = CalculateDonationPeriod(tran.CreateDate),
                            DonateStatus = "Vừa ủng hộ"
                        });
                    }
                }
                return transReponse.Where(t => t.TransactionType == TransactionType.Receive);
            }

        }

        public static string CalculateDonationPeriod(DateTime createDate)
        {
            var timeSpan = TimeHelper.GetTime(DateTime.Now) - createDate;
            if (timeSpan.TotalSeconds < 60)
            {
                return "Vừa mới đây";
            }
            if (timeSpan.TotalMinutes < 60)
            {
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            }
            if (timeSpan.TotalHours < 24)
            {
                return $"{(int)timeSpan.TotalHours} giờ trước";
            }
            if (timeSpan.TotalDays < 7)
            {
                return $"{(int)timeSpan.TotalDays} ngày trước";
            }
            if (timeSpan.TotalDays < 30)
            {
                return $"{(int)timeSpan.TotalDays / 7} tuần trước";
            }
            if (timeSpan.TotalDays < 365)
            {
                return $"{(int)timeSpan.TotalDays / 30} tháng trước";
            }
            return $"{(int)timeSpan.TotalDays / 365} năm trước";
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
                    TransactionType = TransactionType.Receive,
                    Amount = createTransactionRequest.Price,
                    Note = createTransactionRequest.Note,
                    PayerName = "Người ủng hộ ẩn danh",
                    IsIncognito = true,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    AccountId = createTransactionRequest.AccountId,
                    TransactionStatus = TransactionStatus.Pending,
                    TransactionImageUrl = ""
                };
            }
            else
            {
                transaction = new Transaction
                {
                    TransactionID = Guid.NewGuid(),
                    CampaignID = createTransactionRequest.CampaignId,
                    TransactionType = TransactionType.Receive,
                    Amount = createTransactionRequest.Price,
                    Note = createTransactionRequest.Note,
                    PayerName = "",
                    IsIncognito = false,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    AccountId = createTransactionRequest.AccountId,
                    TransactionStatus = TransactionStatus.Pending,
                    TransactionImageUrl = ""
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
            if (paymentLinkInfomation == null)
            {
                throw new Exception("Link thông tin thanh toán bị trống");
            }

            var transaction = _transactionRepository.GetTransactionByOrderId(orderId);
            if (transaction == null)
            {
                throw new NotFoundException("Giao dịch không tìm thấy!");
            }

            var firstTransaction = paymentLinkInfomation.transactions.FirstOrDefault();
            if (paymentLinkInfomation.status.Equals("PAID") && firstTransaction != null && !string.IsNullOrEmpty(firstTransaction.counterAccountName))
            {
                transaction.PayerName = firstTransaction.counterAccountName;
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

                if (transaction == null)
                {
                    throw new NotFoundException("Không tìm thấy giao dịch!");
                }

                if (transaction != null && transaction.TransactionStatus == TransactionStatus.Success)
                {
                    throw new BadRequestException("Giao dịch này đã được kiểm tra, trạng thái giao dịch: thành công! Vui lòng kiểm tra email của bạn!");
                }

                if (transaction != null)
                {
                    if (!transaction.IsIncognito)
                    {
                        transaction.PayerName = request.FirstName + " " + request.LastName;
                    }
                    transaction.TransactionStatus = TransactionStatus.Success;
                    _transactionRepository.Update(transaction);


                    var campaign = _campaignRepository.GetById(transaction.CampaignID);
                    EmailSupporter.SendEmailWithSuccessDonate(request.Email, request.FirstName + " " + request.LastName, campaign!.Name!, transaction.Amount, transaction.CreateDate, campaign.CampaignID);

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


        public IEnumerable<TransactionWithCampaignNameResponse?> GetTransactionByAccountId(Guid accountId, string? transactionStatus)
        {
            var transactions = _transactionRepository.GetHistoryTransactionByAccountId(accountId);

            var transactionsResponse = new List<TransactionWithCampaignNameResponse>();
            foreach (var transaction in transactions)
            {
                if (transaction != null && transaction.Account != null)
                {
                    transaction.Account.Notifications = null;
                    transaction.Account.Transactions = null;
                    transaction.Account.BankingAccounts = null;
                    transaction.Account.AccountTokens = null;
                }

                if (transaction != null && transaction.Campaign != null)
                {
                    transaction.Campaign.Transactions = null;
                    transaction.Campaign.Organization = null;
                    transaction.Campaign.DonatePhase = null;
                    transaction.Campaign.ProcessingPhase = null;
                    transaction.Campaign.CampaignType = null;
                    transaction.Campaign.StatementPhase = null;

                }


                if (transaction != null && transaction.BankingAccount != null)
                {
                    transaction.BankingAccount.Transactions = null;
                    transaction.BankingAccount.Account = null;
                }
                if (!String.IsNullOrEmpty(transactionStatus) && transactionStatus.Equals("PAID"))
                {
                    if (transaction != null && transaction.TransactionStatus == TransactionStatus.Success)
                    {
                        var campaign = _campaignRepository.GetById(transaction.CampaignID);
                        transactionsResponse.Add(new TransactionWithCampaignNameResponse
                        {
                            TransactionID = transaction.TransactionID,
                            CreateDate = transaction.CreateDate,
                            CampaignName = campaign!.Name,
                            AccountId = accountId,
                            Amount = transaction.Amount,
                            BankingAccount = transaction.BankingAccount,
                            Account = transaction.Account,
                            BankingAccountID = transaction.BankingAccountID,
                            Campaign = transaction.Campaign,
                            CampaignID = transaction.CampaignID,
                            IsIncognito = transaction.IsIncognito,
                            Note = transaction.Note,
                            OrderId = transaction.OrderId,
                            PayerName = transaction.PayerName,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            TransactionStatus = transaction.TransactionStatus,
                            TransactionType = transaction.TransactionType
                        });
                    }
                }
                else if (!String.IsNullOrEmpty(transactionStatus) && transactionStatus.Equals("PENDING"))
                {
                    if (transaction != null && transaction.TransactionStatus == TransactionStatus.Pending)
                    {
                        var campaign = _campaignRepository.GetById(transaction.CampaignID);
                        transactionsResponse.Add(new TransactionWithCampaignNameResponse
                        {
                            TransactionID = transaction.TransactionID,
                            CreateDate = transaction.CreateDate,
                            CampaignName = campaign!.Name,
                            AccountId = accountId,
                            Amount = transaction.Amount,
                            BankingAccount = transaction.BankingAccount,
                            Account = transaction.Account,
                            BankingAccountID = transaction.BankingAccountID,
                            Campaign = transaction.Campaign,
                            CampaignID = transaction.CampaignID,
                            IsIncognito = transaction.IsIncognito,
                            Note = transaction.Note,
                            OrderId = transaction.OrderId,
                            PayerName = transaction.PayerName,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            TransactionStatus = transaction.TransactionStatus,
                            TransactionType = transaction.TransactionType
                        });
                    }
                }
                else
                {
                    if (transaction != null)
                    {
                        var campaign = _campaignRepository.GetById(transaction.CampaignID);
                        transactionsResponse.Add(new TransactionWithCampaignNameResponse
                        {
                            TransactionID = transaction.TransactionID,
                            CreateDate = transaction.CreateDate,
                            CampaignName = campaign!.Name,
                            AccountId = accountId,
                            Amount = transaction.Amount,
                            BankingAccount = transaction.BankingAccount,
                            Account = transaction.Account,
                            BankingAccountID = transaction.BankingAccountID,
                            Campaign = transaction.Campaign,
                            CampaignID = transaction.CampaignID,
                            IsIncognito = transaction.IsIncognito,
                            Note = transaction.Note,
                            OrderId = transaction.OrderId,
                            PayerName = transaction.PayerName,
                            TransactionImageUrl = transaction.TransactionImageUrl,
                            TransactionStatus = transaction.TransactionStatus,
                            TransactionType = transaction.TransactionType
                        });
                    }
                }
            }
            return transactionsResponse;
        }



        private void TryValidateRequest(CreateTransactionRequest createTransactionRequest)
        {
            var campaign = _campaignRepository.GetById(createTransactionRequest.CampaignId);
            if (campaign == null)
            {
                throw new Exception("Chiến dịch này không tìm thấy!");
            }

            var account = _accountRepository.GetById(createTransactionRequest.AccountId);
            if (account == null)
            {
                throw new Exception("Tài khoản không tìm thấy!");
            }

            if (createTransactionRequest.Price < 2000)
            {
                throw new Exception("Số tiền quyên góp phải lớn hơn hoặc bằng 2000 đồng!");
            }

        }

        public async Task<string> GetData(int orderId)
        {
            string url = "https://api-merchant.payos.vn/v2/payment-requests/" + orderId;
            HttpClient httpClient = new HttpClient();
            JObject responseBodyJson = JObject.Parse(await (await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers =
            {
                { "x-client-id", PayOSConstants.ClientId },
                { "x-api-key", PayOSConstants.ApiKey }
            }
            })).Content.ReadAsStringAsync());
            string code = responseBodyJson["code"]?.ToString();
            string desc = responseBodyJson["desc"]?.ToString();
            string data = responseBodyJson["data"]?.ToString();


            JObject dataJson = JObject.Parse(data);
            string paymentLinkResSignature = SignatureControl.CreateSignatureFromObj(dataJson, PayOSConstants.CheckSumKey);
            return data + " " + responseBodyJson["signature"].ToString();
        }


        public async Task<string?> CheckTransactionCNTAsync(int orderId)
        {
            var transaction = _transactionRepository.GetTransactionByOrderId(orderId);
            if (transaction == null)
            {
                throw new NotFoundException("Giao dịch không tìm thấy!");
            }

            var account = new RequestAccountLoginCNT
            {
                email = "chaunhattruong4747@gmail.com",
                password = "a80696a59010bd22c5fab52893c3aafd1f228e205fdf548defa51041e167ac7dcfbfe12f23f1ec5a1ae8abdc192ce0466b3d17c626b458d32723c9a3ff3652eb"
            };


            //var json = JsonConvert.SerializeObject(account);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseString = await PostAPI("https://api-app.payos.vn/auth/sign-in", "", account);
            ApiResponse response = new ApiResponse();
            response = JsonConvert.DeserializeObject<ApiResponse>(responseString)!;
            var responseListOrder = await GetAPI("https://api-app.payos.vn/organizations/838ab7571bd411ef915f0242ac110002/statistics/payment-link?page=0&pageSize=&typeOrder=", response.Data.Token);
            Root listDataPayos = new Root();
            listDataPayos = JsonConvert.DeserializeObject<Root>(responseListOrder)!;
            if (listDataPayos != null)
            {
                if (listDataPayos.data == null) throw new BadRequestException("Danh sách data của payos bị trống");
                foreach (var item in listDataPayos.data.orders)
                {
                    if (item.order_code == orderId)
                    {
                        return item.status;
                    }
                }
            }
            return null;
        }


        public async Task<string?> CheckAndSendEmailWithSuccessStatusCNTAsync(CheckTransactionRequest? request)
        {
            if (request == null) throw new BadRequestException("Dữ liệu truyền xuống không được để trống!");
            var status = await CheckTransactionCNTAsync(request.OrderID);
            if (!String.IsNullOrEmpty(status) && status.ToLower().Contains("PAID".ToLower()))
            {
                var transaction = _transactionRepository.GetTransactionByOrderId(request.OrderID);

                if (transaction == null)
                {
                    throw new NotFoundException("Không tìm thấy giao dịch!");
                }

                if (transaction != null && transaction.TransactionStatus == TransactionStatus.Success)
                {
                    throw new BadRequestException("Giao dịch này đã được kiểm tra, trạng thái giao dịch: thành công! Vui lòng kiểm tra email của bạn!");
                }

                if (transaction != null)
                {
                    if (!transaction.IsIncognito)
                    {
                        transaction.PayerName = request.FirstName.ToUpper() + " " + request.LastName.ToUpper();
                    }
                    transaction.TransactionStatus = TransactionStatus.Success;
                    _transactionRepository.Update(transaction);


                    var campaign = _campaignRepository.GetById(transaction.CampaignID);
                    EmailSupporter.SendEmailWithSuccessDonate(request.Email, request.FirstName.ToUpper() + " " + request.LastName.ToUpper(), campaign != null && campaign.Name != null ? campaign.Name : "Chiến dịch thiện nguyện của trang chủ VMO", transaction.Amount, transaction.CreateDate, transaction.CampaignID);

                    _donatePhaseService.UpdateDonatePhaseByCampaignIdAndAmountDonate(transaction.CampaignID, transaction.Amount);
                    _transactionRepository.Update(transaction);
                }
            }
            return status;
        }

        public async Task<Transaction?> UploadImageTransactionByAdmin(CreateTransactionWithUploadImage request)
        {
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null)
            {
                throw new NotFoundException("Người dùng không tìm thấy!");
            }
            var campaign = _campaignRepository.GetById(request.CampaignId);
            if (campaign == null)
            {
                throw new NotFoundException("Chiến dịch không tìm thấy!");
            }
            var bankingAccount = _bankingAccountRepository.GetById(request.BankingAccountId);
            if (bankingAccount == null)
            {
                throw new NotFoundException("Tài khoản ngân hàng không tìm thấy!");
            }
            var transaction = new Transaction
            {
                TransactionID = Guid.NewGuid(),
                TransactionStatus = TransactionStatus.Success,
                AccountId = request.AccountId,
                BankingAccountID = request.BankingAccountId,
                CampaignID = request.CampaignId,
                TransactionType = TransactionType.Transfer,
                Amount = request.Amount,
                Note = "Chuyển tiền giải ngân",
                PayerName = "ADMIN",
                IsIncognito = false,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                TransactionImageUrl = await _firebaseService.UploadImage(request.TransactionImage)
            };
            return _transactionRepository.Save(transaction);
        }





        public async Task<string> PostAPI(string url, string token, object obj)
        {
            var client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(url);
            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            //request.Headers.Add("Bearer", token);
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        public async Task<string> GetAPI(string url, string token)
        {
            var client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(url);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            //request.Headers.Add("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
