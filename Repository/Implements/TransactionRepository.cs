using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Enums;

namespace Repository.Implements
{
    public class TransactionRepository : ITransactionRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetAll()
        {
            using var context = new VMODBContext();
            return context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).OrderByDescending(a => a.CreateDate).ToList();
        }

        public Transaction? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.TransactionID.Equals(id));
        }

        public IEnumerable<Transaction?> GetHistoryTransactionByAccountId(Guid accountId)
        {
            using var context = new VMODBContext();
            return context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).ToList().Where(t => t.AccountId.Equals(accountId));
        }

        public Transaction? GetTransactionByCampaignIdWithTypeIsTransfer(Guid campaignId)
        {
            using var context = new VMODBContext();
            return context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.CampaignID.Equals(campaignId) && d.TransactionType == TransactionType.Transfer);
        }

        public IEnumerable<Transaction>? GetTransactionByCampaignTierIIIdWithTypeIsTransfer(Guid campaignId)
        {
            using var context = new VMODBContext();
            return context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).ToList()
                .Where(d => d.CampaignID.Equals(campaignId) && d.TransactionType == TransactionType.Transfer);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionReceiveForStatementByAdminAsync(string? campaignName, int? pageSize, int? pageNo)
        {
            await using var context = new VMODBContext();
            var query = context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).OrderByDescending(a => a.CreateDate).ToList()
                .Where(o =>
                    o.Campaign?.Name != null && o.Campaign != null && o.TransactionType == TransactionType.Receive && o.TransactionStatus == TransactionStatus.Success && o.Campaign.Name.ToLower().Contains(campaignName?.ToLower().Trim() ?? string.Empty));
            int totalCount = query.Count();

            // Set pageSize to the total count if it's not provided
            int size = pageSize ?? totalCount;
            int page = pageNo ?? 1;

            // Apply pagination
            return query
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

        }

        public async Task<IEnumerable<Transaction>> GetTransactionReceiveForStatementByAdminAsync(string? campaignName)
        {
            await using var context = new VMODBContext();
            var query = context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).OrderByDescending(a => a.CreateDate).ToList()
                .Where(o =>
                    o.Campaign?.Name != null && o.Campaign != null && o.TransactionType == TransactionType.Receive && o.TransactionStatus == TransactionStatus.Success && o.Campaign.Name.ToLower().Contains(campaignName?.ToLower().Trim() ?? string.Empty));
            return query.ToList();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionSendForStatementByAdminAsync(string? campaignName)
        {
            await using var context = new VMODBContext();
            var query = context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).OrderByDescending(a => a.CreateDate).ToList()
                .Where(o =>
                    o.Campaign?.Name != null && o.Campaign != null && o.TransactionType == TransactionType.Transfer && o.TransactionStatus == TransactionStatus.Success && o.Campaign.Name.ToLower().Contains(campaignName?.ToLower().Trim() ?? string.Empty));
            return query.ToList();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionSendForStatementByAdminAsync(string? campaignName, int? pageSize, int? pageNo)
        {
            await using var context = new VMODBContext();
            var query = context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).OrderByDescending(a => a.CreateDate).ToList()
                .Where(o =>
                    o.Campaign?.Name != null && o.Campaign != null && o.TransactionType == TransactionType.Transfer && o.TransactionStatus == TransactionStatus.Success && o.Campaign.Name.ToLower().Contains(campaignName?.ToLower().Trim() ?? string.Empty));
            int totalCount = query.Count();

            // Set pageSize to the total count if it's not provided
            int size = pageSize ?? totalCount;
            int page = pageNo ?? 1;

            // Apply pagination
            return query
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

        }

        public Transaction? GetTransactionByProcessingPhaseIdWithTypeIsTransfer(Guid processingPhaseId)
        {
            using var context = new VMODBContext();
            return context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.ProcessingPhaseId.Equals(processingPhaseId) && d.TransactionType == TransactionType.Transfer);
        }

        public IEnumerable<Transaction>? GetTransactionsByProcessingPhaseIdWithTypeIsTransfer(Guid processingPhaseId)
        {
            using var context = new VMODBContext();
            return context.Transactions.ToList()
                .Where(d => d.ProcessingPhaseId.Equals(processingPhaseId) && d.TransactionType == TransactionType.Transfer);
        }

        public IEnumerable<Transaction>? GetTransactionByCampaignId(Guid campaignId)
        {
            using var context = new VMODBContext();
            return context.Transactions.ToList()
                .Where(d => d.CampaignID.Equals(campaignId));
        }

        public Transaction? GetTransactionByOrderId(int orderId)
        {
            using var context = new VMODBContext();
            return context.Transactions
                .Include(a => a.Account)
                .Include(a => a.BankingAccount)
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.OrderId.Equals(orderId));
        }

        public Transaction? Save(Transaction entity)
        {
            try
            {
                using var context = new VMODBContext();
                var transactionCreated = context.Transactions.Add(entity);
                context.SaveChanges();
                return transactionCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Transaction entity)
        {
            try
            {
                using var context = new VMODBContext();
                context.Entry(entity).Property(p => p.OrderId).IsModified = false;
                context.Entry(entity).Property(p => p.PayerName).IsModified = true;
                context.Entry(entity).Property(p => p.TransactionStatus).IsModified = true;

                // Add similar lines for other fields you want to update
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}
