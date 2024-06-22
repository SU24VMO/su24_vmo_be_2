using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implements
{
    public class OrganizationManagerRepository : IOrganizationManagerRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrganizationManager> GetAll()
        {
            using var context = new VMODBContext();
            return context.OrganizationManagers
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateCampaignRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.Organizations)
                .Include(a => a.BankingAccounts).ToList();
        }

        public OrganizationManager? GetByAccountID(Guid accountID)
        {
            using var context = new VMODBContext();
            return context.OrganizationManagers
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateCampaignRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.Organizations)
                .Include(a => a.BankingAccounts).ToList()
                .FirstOrDefault(d => d.AccountID.Equals(accountID));
        }

        public OrganizationManager? GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public OrganizationManager? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.OrganizationManagers
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateCampaignRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.Organizations)
                .Include(a => a.BankingAccounts).ToList()
                .FirstOrDefault(d => d.OrganizationManagerID.Equals(id));
        }

        public OrganizationManager? GetByPhone(string phone)
        {
            using var context = new VMODBContext();
            return context.OrganizationManagers
                .Include(a => a.CreateOrganizationRequests)
                .Include(a => a.CreateCampaignRequests)
                .Include(a => a.CreatePostRequests)
                .Include(a => a.Organizations)
                .Include(a => a.BankingAccounts).ToList()
                .FirstOrDefault(d => d.PhoneNumber.Equals(phone));
        }

        public OrganizationManager? Save(OrganizationManager entity)
        {
            using var context = new VMODBContext();
            //var mytransaction = context.Database.BeginTransaction();
            try
            {
                var account = entity.Account;
                account!.Email = account.Email.ToLower();
                var accountAdded = context.Accounts.Add(account);
                var userAdded = context.OrganizationManagers.Add(entity);
                context.SaveChanges();
                //mytransaction.Commit();
                return userAdded.Entity;
            }
            catch
            {
                //mytransaction.Rollback();
                throw;
            }
        }

        public void Update(OrganizationManager entity)
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
