using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
