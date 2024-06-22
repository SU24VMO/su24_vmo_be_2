using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class BankingAccountService
    {
        private readonly IBankingAccountRepository _bankingAccountRepository;
        private readonly IAccountRepository _accountRepository;

        public BankingAccountService(IBankingAccountRepository bankingAccountRepository, IAccountRepository accountRepository)
        {
            _bankingAccountRepository = bankingAccountRepository;
            _accountRepository = accountRepository;
        }

        public IEnumerable<BankingAccount> GetAll()
        {
            return _bankingAccountRepository.GetAll();
        }

        public BankingAccount? GetById(Guid id)
        {
            return _bankingAccountRepository.GetById(id);
        }

        public BankingAccount? CreateBankingAccount(CreateBankingAccountRequest request)
        {
            TryValidateCreateBankingAccountRequest(request);
            var bankingAccount = new BankingAccount
            {
                BankingAccountID = Guid.NewGuid(),
                BankingName = request.BankingName,
                AccountName = request.AccountName,
                QRCode = request.QRCode,
                AccountNumber = request.AccountNumber,
                AccountId = request.AccountId,
                CreatedAt = TimeHelper.GetTime(DateTime.UtcNow),
                IsAvailable = true
            };

            return _bankingAccountRepository.Save(bankingAccount);
        }

        public void UpdateBankingAccount(UpdateBankingAccountRequest request)
        {
            TryValidateUpdateBankingAccountRequest(request);
            var bankingAccount = _bankingAccountRepository.GetById(request.BankingAccountID)!;
            if (!String.IsNullOrEmpty(bankingAccount.AccountNumber))
            {
                bankingAccount.AccountNumber = request.AccountNumber!.Trim();
            }
            if (!String.IsNullOrEmpty(bankingAccount.AccountName))
            {
                bankingAccount.AccountName = request.AccountName!.Trim();
            }
            if (!String.IsNullOrEmpty(bankingAccount.BankingName))
            {
                bankingAccount.BankingName = request.BankingName!.Trim();
            }
            if (!String.IsNullOrEmpty(bankingAccount.QRCode))
            {
                bankingAccount.QRCode = request.QRCode!.Trim();
            }
            if (request.IsAvailable != null)
            {
                bankingAccount.IsAvailable = (bool)request.IsAvailable;
            }
            bankingAccount.UpdatedAt = TimeHelper.GetTime(DateTime.UtcNow);
            _bankingAccountRepository.Update(bankingAccount);
        }

        private void TryValidateCreateBankingAccountRequest(CreateBankingAccountRequest request)
        {
            if (!String.IsNullOrEmpty(request.AccountId.ToString()))
            {
                throw new Exception("AccountId must not empty.");
            }

            if (_accountRepository.GetById(request.AccountId) == null)
            {
                throw new Exception("Account not found.");
            }
            if (!String.IsNullOrEmpty(request.AccountNumber))
            {
                throw new Exception("AccountNumber is not empty.");
            }
            if (!String.IsNullOrEmpty(request.AccountName))
            {
                throw new Exception("AccountName is not empty.");
            }
            if (!String.IsNullOrEmpty(request.BankingName))
            {
                throw new Exception("BankingName is not empty.");
            }
            if (!String.IsNullOrEmpty(request.QRCode))
            {
                throw new Exception("QRCode is not empty.");
            }
        }

        private void TryValidateUpdateBankingAccountRequest(UpdateBankingAccountRequest request)
        {
            if (!String.IsNullOrEmpty(request.BankingAccountID.ToString()))
            {
                throw new Exception("BankingAccountID must not empty.");
            }

            if (_bankingAccountRepository.GetById(request.BankingAccountID) == null)
            {
                throw new Exception("BankingAccount not found.");
            }
        }
    }
}
