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
    public class CampaignRepository : ICampaignRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Campaign> GetAll()
        {
            using var context = new VMODBContext();
            return context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .OrderByDescending(a => a.CreateAt).ToList();
        }

        public IEnumerable<Campaign> GetAll(int pageNumber, int pageSize)
        {
            using var context = new VMODBContext();
            return context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .OrderByDescending(a => a.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Campaign? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase).ToList()
                .FirstOrDefault(d => d.CampaignID.Equals(id));
        }

        public IEnumerable<Campaign> GetCampaignsByCampaignName(string campaignName)
        {
            using var context = new VMODBContext();
            return context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .OrderByDescending(a => a.CreateAt).ToList().Where(a => a.Name!.ToLower().Contains(campaignName.ToLower()));
        }

        public IEnumerable<Campaign> GetCampaignsByCampaignTypeId(Guid campaignTypeId)
        {
            using var context = new VMODBContext();
            return context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .OrderByDescending(a => a.CreateAt).ToList().Where(a => a.CampaignTypeID!.Equals(campaignTypeId));
        }

        public Campaign? Save(Campaign entity)
        {
            try
            {
                using var context = new VMODBContext();
                var campaignCreated = context.Campaigns.Add(entity);
                context.SaveChanges();
                return campaignCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Campaign entity)
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
