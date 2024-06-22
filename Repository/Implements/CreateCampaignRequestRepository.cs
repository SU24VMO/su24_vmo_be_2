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
                .Include(a => a.User)
                .Include(a => a.RequestManager).ToList();
        }

        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByMemberId(Guid userId)
        {
            using var context = new VMODBContext();
            return context.CreateCampaignRequests
                .Include(a => a.Campaign)
                .Include(a => a.OrganizationManager)
                .Include(a => a.User)
                .Include(a => a.RequestManager).ToList().Where(c => c.CreateByUser.Equals(userId));
        }

        public IEnumerable<CreateCampaignRequest> GetAllCreateCampaignRequestByOrganizationManagerId(Guid organizationManagerId)
        {
            using var context = new VMODBContext();
            return context.CreateCampaignRequests
                .Include(a => a.Campaign)
                .Include(a => a.OrganizationManager)
                .Include(a => a.User)
                .Include(a => a.RequestManager).ToList().Where(c => c.CreateByOM.Equals(organizationManagerId));
        }

        public CreateCampaignRequest? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.CreateCampaignRequests
                .Include(a => a.Campaign)
                .Include(a => a.OrganizationManager)
                .Include(a => a.User)
                .Include(a => a.RequestManager).ToList().
                    FirstOrDefault(a => a.CreateCampaignRequestID.Equals(id));
        }

        public CreateCampaignRequest? GetCreateCampaignRequestByCampaignId(Guid campaignId)
        {
            using var context = new VMODBContext();
            return context.CreateCampaignRequests
                .Include(a => a.Campaign)
                .Include(a => a.OrganizationManager)
                .Include(a => a.User)
                .Include(a => a.RequestManager).ToList().
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
                    // Update existing banking account if necessary, without modifying the key

                    bankingAccountExisted.BankingName = bankingAccount.BankingName;

                    bankingAccountExisted.AccountName = bankingAccount.AccountName;

                    bankingAccountExisted.CreatedAt = bankingAccount.CreatedAt;

                    bankingAccountExisted.IsAvailable = bankingAccount.IsAvailable;

                    bankingAccountExisted.QRCode = bankingAccount.QRCode;

                    // No need to set AccountId or BankingAccountID as they should remain unchanged
                }

                // Add the CreateCampaignRequest
                entity.User = null;
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
