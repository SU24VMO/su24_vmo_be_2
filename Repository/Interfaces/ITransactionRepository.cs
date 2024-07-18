using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ITransactionRepository : ICrudBaseRepository<Transaction, Guid>
    {
        public Transaction? GetTransactionByOrderId(int orderId);
        public IEnumerable<Transaction?> GetHistoryTransactionByAccountId(Guid accountId);
        public Transaction? GetTransactionByCampaignIdWithTypeIsTransfer(Guid campaignId);
    }
}
