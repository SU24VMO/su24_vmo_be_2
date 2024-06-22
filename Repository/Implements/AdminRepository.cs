using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implements
{
    public class AdminRepository : IAdminRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Admin> GetAll()
        {
            using var context = new VMODBContext();
            return context.Admins
                .Include(a => a.Account).ToList();
        }

        public Admin? GetByAccountID(Guid accountID)
        {
            using var context = new VMODBContext();
            return context.Admins
                .Include(b => b.Account).ToList()
                .FirstOrDefault(d => d.AccountID.Equals(accountID));
        }

        public Admin? GetByEmail(string email)
        {
            using var context = new VMODBContext();
            var account = context.Accounts
                .Include(a => a.Notifications).ToList()
                .FirstOrDefault(d => d.Email.Equals(email.Trim()));
            return account != null ? (context.Admins.FirstOrDefault(a => a.AccountID.Equals(account.AccountID))) : null;
        }

        public Admin? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Admins
                .Include(a => a.Account).ToList()
                .FirstOrDefault(d => d.AdminID.Equals(id));
        }

        public Admin? Save(Admin entity)
        {
            try
            {
                using var context = new VMODBContext();
                var adminCreated = context.Admins.Add(entity);
                context.SaveChanges();
                return adminCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Admin entity)
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
