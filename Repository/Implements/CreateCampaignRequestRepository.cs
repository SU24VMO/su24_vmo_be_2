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
    public class CreateCampaignRequestRepository : ICreateCampaignRequestRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CreateCampaignRequest> GetAll()
        {
            using var context = new VMODBContext();
            return context.CreateCampaignRequests
                .Include(a => a.Campaign)
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByVolunteerId(Guid memberId)
        {
            using var context = new VMODBContext();
            return context.CreateCampaignRequests
                .Include(a => a.Campaign)
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList().Where(c => c.CreateByMember.Equals(memberId));
        }

        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByOrganizationManagerId(Guid organizationManagerId)
        {
            using var context = new VMODBContext();
            return context.CreateCampaignRequests
                .Include(a => a.Campaign)
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator)
                .OrderByDescending(a => a.CreateDate).ToList().Where(c => c.CreateByOM.Equals(organizationManagerId));
        }

        public CreateCampaignRequest? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.CreateCampaignRequests
                .Include(a => a.Campaign)
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator).ToList().
                    FirstOrDefault(a => a.CreateCampaignRequestID.Equals(id));
        }

        public CreateCampaignRequest? GetCreateCampaignRequestByCampaignId(Guid campaignId)
        {
            using var context = new VMODBContext();
            return context.CreateCampaignRequests
                .Include(a => a.Campaign)
                .Include(a => a.OrganizationManager)
                .Include(a => a.Member)
                .Include(a => a.Moderator).ToList().
                    FirstOrDefault(a => a.CampaignID.Equals(campaignId));
        }

        public CreateCampaignRequest? Save(CreateCampaignRequest entity)
        {
            using var context = new VMODBContext();
            var mytransaction = context.Database.BeginTransaction();
            try
            {
                var campaign = entity.Campaign;
                var createCampaignRequest = context.CreateCampaignRequests.Add(entity);
                var userAdded = context.Campaigns.Add(campaign!);
                context.SaveChanges();
                mytransaction.Commit();
                return createCampaignRequest.Entity;
            }
            catch
            {
                mytransaction.Rollback();
                throw;
            }
        }

        public CreateCampaignRequest? SaveWithBankingAccount(CreateCampaignRequest entity, BankingAccount bankingAccount)
        {
            using var context = new VMODBContext();
            var mytransaction = context.Database.BeginTransaction();
            try
            {
                var campaign = entity.Campaign;
                var existingCampaign = context.Campaigns.FirstOrDefault(c => c.CampaignID == campaign!.CampaignID);
                // Check if the banking account already exists
                var bankingAccountExisted = context.BankingAccounts
                    .FirstOrDefault(b => b.AccountNumber.Equals(bankingAccount.AccountNumber));
                if (bankingAccountExisted == null)
                {
                    // Add the new banking account
                    //bankingAccount.Account = null;
                    context.BankingAccounts.Add(bankingAccount);
                }
                else
                {
                    campaign!.BankingAccountID = bankingAccountExisted.BankingAccountID;
                }

                if (existingCampaign == null)
                {
                    // Add the new campaign
                    context.Campaigns.Add(campaign!);
                }
                else
                {
                    // Update existing campaign if necessary
                    context.Entry(existingCampaign).CurrentValues.SetValues(campaign!);
                }



                

                // Add the CreateCampaignRequest
                entity.Member = null;
                var createCampaignRequest = context.CreateCampaignRequests.Add(entity);

                // Save all changes to the database
                context.SaveChanges();
                mytransaction.Commit();
                return createCampaignRequest.Entity;
            }
            catch
            {
                mytransaction.Rollback();
                throw;
            }
        }

        public async Task<CreateCampaignRequest?> SaveWithBankingAccountAsync(CreateCampaignRequest entity, BankingAccount bankingAccount)
        {
            using var context = new VMODBContext();
            using var mytransaction = await context.Database.BeginTransactionAsync();
            try
            {
                var campaign = entity.Campaign;
                var existingCampaign = await context.Campaigns.FirstOrDefaultAsync(c => c.CampaignID == campaign!.CampaignID);

                var bankingAccountExisted = await context.BankingAccounts
                    .FirstOrDefaultAsync(b => b.AccountNumber.Equals(bankingAccount.AccountNumber));

                if (bankingAccountExisted == null)
                {
                    await context.BankingAccounts.AddAsync(bankingAccount);
                }
                else
                {
                    campaign!.BankingAccountID = bankingAccountExisted.BankingAccountID;
                }

                if (existingCampaign == null)
                {
                    await context.Campaigns.AddAsync(campaign!);
                }
                else
                {
                    context.Entry(existingCampaign).CurrentValues.SetValues(campaign!);
                }

                entity.Member = null;
                await context.CreateCampaignRequests.AddAsync(entity);

                await context.SaveChangesAsync();
                await mytransaction.CommitAsync();
                return entity;
            }
            catch
            {
                await mytransaction.RollbackAsync();
                throw;
            }
        }

        public void Update(CreateCampaignRequest entity)
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
