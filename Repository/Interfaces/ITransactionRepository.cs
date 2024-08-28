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
        public IEnumerable<Transaction> GetTransactionByCampaignTierIIIdWithTypeIsTransfer(Guid campaignId);
        public Task<IEnumerable<Transaction>> GetTransactionReceiveForStatementByAdminAsync(string? campaignName, int? pageSize, int? pageNo);
        public Task<IEnumerable<Transaction>> GetTransactionReceiveForStatementByAdminAsync(string? campaignName);
        public Task<IEnumerable<Transaction>> GetTransactionSendForStatementByAdminAsync(string? campaignName);
        public Task<IEnumerable<Transaction>> GetTransactionSendForStatementByAdminAsync(string? campaignName, int? pageSize, int? pageNo);
        public Transaction? GetTransactionByProcessingPhaseIdWithTypeIsTransfer(Guid processingPhaseId);



    }
}
