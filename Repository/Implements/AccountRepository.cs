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
                .Include(a => a.Notifications)
                .OrderByDescending(a => a.CreatedAt).ToList();
        }

        public IEnumerable<Account> GetAllAccountsWithVolunteerRole(string? name)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.Username != null && a.Role.Equals(BusinessObject.Enums.Role.Volunteer) && a.Username.ToLower().Contains(name?.ToLower().Trim() ?? string.Empty));
        }

        public IEnumerable<Account> GetAllAccountsWithVolunteerRole(string? name, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.Username != null && a.Role.Equals(BusinessObject.Enums.Role.Volunteer) && a.Username.ToLower().Contains(name?.ToLower().Trim() ?? string.Empty));
        }

        public IEnumerable<Account> GetAllAccountsWithOrganizationManagerRole(string? name)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.Username != null && a.Role.Equals(BusinessObject.Enums.Role.OrganizationManager) && a.Username.ToLower().Contains(name?.ToLower().Trim() ?? string.Empty));
        }
        public IEnumerable<Account> GetAllAccountsWithOrganizationManagerRole(string? name, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.Username != null && a.Role.Equals(BusinessObject.Enums.Role.OrganizationManager) && a.Username.ToLower().Contains(name?.ToLower().Trim() ?? string.Empty));
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            await using var context = new VMODBContext();
            return await context.Accounts
                .Include(a => a.Notifications)
                .Include(a => a.Transactions)
                .Include(a => a.BankingAccounts)
                .FirstOrDefaultAsync(d => d.AccountID.Equals(id));
        }

        public IEnumerable<Account> GetAllAccountsWithModeratorRole(string? name)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.Username != null && a.Role.Equals(BusinessObject.Enums.Role.Moderator) && a.Username.ToLower().Contains(name?.ToLower().Trim() ?? string.Empty));
        }

        public IEnumerable<Account> GetAllAccountsWithModeratorRole(string? name, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.Username != null && a.Role.Equals(BusinessObject.Enums.Role.Moderator) && a.Username.ToLower().Contains(name?.ToLower().Trim() ?? string.Empty));
        }

        public IEnumerable<Account> GetAllAccountWithMemberRole(string? name)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.Username != null && a.Role.Equals(BusinessObject.Enums.Role.Member) && a.Username.ToLower().Contains(name?.ToLower().Trim() ?? string.Empty));
        }

        public IEnumerable<Account> GetAllAccountWithMemberRole(string? name, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            return context.Accounts
                .OrderByDescending(a => a.CreatedAt).ToList().Where(a => a.Username != null && a.Role.Equals(BusinessObject.Enums.Role.Member) && a.Username.ToLower().Contains(name?.ToLower().Trim() ?? string.Empty));
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
                .Include(a => a.Notifications)
                .Include(a => a.Transactions)
                .Include(a => a.BankingAccounts)
                .ToList()
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
