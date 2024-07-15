using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IBankingAccountRepository : ICrudBaseRepository<BankingAccount, Guid>
    {
        public IEnumerable<BankingAccount> GetBankingAccountsByAccountId(Guid accountId);
        public BankingAccount? GetBankingAccountByCampaignId(Guid campaignId);
    }
}
