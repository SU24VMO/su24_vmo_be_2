using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implements
{
    public class MemberRepository : IMemberRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Member> GetAll()
        {
            using var context = new VMODBContext();
            return context.Members
                .Include(b => b.BankingAccounts)
                .Include(c => c.Account)
                .Include(c => c.CreateCampaignRequests).ToList();
        }

        public Member? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Members
                .Include(b => b.BankingAccounts)
                .Include(c => c.Account)
                .Include(c => c.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.MemberID.Equals(id));
        }

        public Member? GetByAccountId(Guid? accountId)
        {
            using var context = new VMODBContext();
            return context.Members
                .Include(b => b.BankingAccounts)
                .Include(c => c.Account)
                .Include(c => c.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.AccountID.Equals(accountId));
        }

        public Member? Save(Member user)
        {
            using var context = new VMODBContext();
            var mytransaction = context.Database.BeginTransaction();
            try
            {
                var userAdded = context.Members.Add(user);
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

        public void Update(Member entity)
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


        public Member? GetByPhone(string phone)
        {
            using var context = new VMODBContext();
            return context.Members
                .Include(b => b.BankingAccounts)
                .Include(c => c.Account)
                .Include(c => c.CreateCampaignRequests).ToList()
                .FirstOrDefault(d => d.PhoneNumber != null && d.PhoneNumber.Equals(phone));
        }

        public async Task<Member?> GetByAccountIdAsync(Guid? accountId)
        {
            await using var context = new VMODBContext();
            return await context.Members
                .Include(b => b.BankingAccounts)
                .Include(c => c.Account)
                .Include(c => c.CreateCampaignRequests)
                .FirstOrDefaultAsync(d => d.AccountID.Equals(accountId));
        }
    }
}
