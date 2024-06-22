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
    public class AccountRepository : IAccountRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Account> GetAll()
        {
            using var context = new VMODBContext();
            return context.Accounts
                .Include(a => a.Transactions)
                .Include(a => a.Notifications).ToList();
        }

        public IEnumerable<Account> GetAllAccountsWithMemberRole()
        {
            using var context = new VMODBContext();
            return context.Accounts
                .Include(a => a.Transactions)
                .Include(a => a.Notifications).ToList().Where(a => a.Role.Equals(BusinessObject.Enums.Role.Member));
        }

        public IEnumerable<Account> GetAllAccountsWithOrganizationManagerRole()
        {
            using var context = new VMODBContext();
            return context.Accounts
                .Include(a => a.Transactions)
                .Include(a => a.Notifications).ToList().Where(a => a.Role.Equals(BusinessObject.Enums.Role.OrganizationManager));
        }

        public IEnumerable<Account> GetAllAccountsWithRequestManagerRole()
        {
            using var context = new VMODBContext();
            return context.Accounts
                .Include(a => a.Transactions)
                .Include(a => a.Notifications).ToList().Where(a => a.Role.Equals(BusinessObject.Enums.Role.RequestManager));
        }

        public IEnumerable<Account> GetAllAccountsWithUserRole()
        {
            using var context = new VMODBContext();
            return context.Accounts
                .Include(a => a.Transactions)
                .Include(a => a.Notifications).ToList().Where(a => a.Role.Equals(BusinessObject.Enums.Role.User));
        }

        public Account? GetByEmail(string email)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .Include(a => a.Notifications).ToList()
                .FirstOrDefault(d => d.Email.ToLower().Equals(email.Trim().ToLower()));
        }

        public Account? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .Include(a => a.Notifications).ToList()
                .FirstOrDefault(d => d.AccountID.Equals(id));
        }

        public Account? GetByUsername(string username)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .Include(a => a.Notifications).ToList()
                .FirstOrDefault(d => d.Username != null && d.Username.ToLower().Equals(username!.Trim().ToLower()));
        }

        public Account? Save(Account entity)
        {
            try
            {
                using var context = new VMODBContext();
                entity.Email = entity.Email.ToLower();
                var userAdded = context.Accounts.Add(entity);
                context.SaveChanges();
                return userAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Account entity)
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
