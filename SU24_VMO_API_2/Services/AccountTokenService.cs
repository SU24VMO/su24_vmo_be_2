using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;

namespace SU24_VMO_API.Services
{
    public class AccountTokenService
    {
        private readonly IAccountTokenRepository _accountTokenRepository;

        public AccountTokenService(IAccountTokenRepository accountTokenRepository)
        {
            _accountTokenRepository = accountTokenRepository;
        }

        public AccountToken? AddAccountToken(AccountToken accountToken)
        {
            return _accountTokenRepository.Save(accountToken);
        }

        public IEnumerable<AccountToken> GetAll()
        {
            return _accountTokenRepository.GetAll();
        }
    }
}
