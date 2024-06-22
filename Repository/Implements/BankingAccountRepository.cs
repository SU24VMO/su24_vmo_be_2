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
    public class BankingAccountRepository : IBankingAccountRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BankingAccount> GetAll()
        {
            using var context = new VMODBContext();
            return context.BankingAccounts
                .Include(a => a.Account)
                .Include(a => a.Transactions).ToList();
        }

        public BankingAccount? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.BankingAccounts
                .Include(a => a.Account)
                .Include(a => a.Transactions).ToList()
                .FirstOrDefault(b => b.BankingAccountID.Equals(id));
        }

        public BankingAccount? Save(BankingAccount entity)
        {
            try
            {
                using var context = new VMODBContext();
                var bankingAccountCreated = context.BankingAccounts.Add(entity);
                context.SaveChanges();
                return bankingAccountCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(BankingAccount entity)
        {
            try
            {
                using var context = new VMODBContext();
                context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}
