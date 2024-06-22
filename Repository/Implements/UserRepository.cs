using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implements
{
    public class UserRepository : IUserRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            using var context = new VMODBContext();
            return context.Users
                .Include(b => b.BankingAccounts)
                .Include(c => c.Account)
                .Include(c => c.CreateCampaignRequests).ToList();
        }

        public User? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Users
                .Include(b => b.BankingAccounts)
                .Include(c => c.Account)
                .Include(c => c.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.UserID.Equals(id));
        }

        public User? GetByAccountId(Guid? accountId)
        {
            using var context = new VMODBContext();
            return context.Users
                .Include(b => b.BankingAccounts)
                .Include(c => c.Account)
                .Include(c => c.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.AccountID.Equals(accountId));
        }

        public User? Save(User user)
        {
            using var context = new VMODBContext();
            var mytransaction = context.Database.BeginTransaction();
            try
            {
                var userAdded = context.Users.Add(user);
                context.SaveChanges();
                mytransaction.Commit();
                return userAdded.Entity;
            }
            catch
            {
                mytransaction.Rollback();
                throw;
            }
        }

        public void Update(User entity)
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


        public User? GetByPhone(string phone)
        {
            using var context = new VMODBContext();
            return context.Users
                .Include(b => b.BankingAccounts)
                .Include(c => c.Account)
                .Include(c => c.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.PhoneNumber != null && d.PhoneNumber.Equals(phone));
        }
    }
}
