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

        public IEnumerable<Campaign> GetCampaignsCreateByVolunteer()
        {
            using var context = new VMODBContext();

            // Start with the base query
            var query = context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .Where(c => c.IsActive)
                .AsQueryable();
            return query.Where(c => c.OrganizationID == null).ToList();
        }

        public IEnumerable<Campaign> GetAll(int pageNumber, int pageSize, string? status, Guid? campaignTypeId, string? createBy, string? campaignName)
        {
            using var context = new VMODBContext();

            // Start with the base query
            var query = context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .Where(c => c.IsActive)
                .AsQueryable();

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower().Equals("đang thực hiện"))
                {
                    query = query.Where(c => c.IsActive && !c.IsComplete);
                }
                else if (status.ToLower().Equals("đạt mục tiêu"))
                {
                    query = query.Where(c => c.DonatePhase != null && c.DonatePhase.IsEnd);
                }
                else if (status.ToLower().Equals("đã kết thúc"))
                {
                    query = query.Where(c => c.IsComplete);
                }
            }

            // Apply campaignTypeId filter
            if (campaignTypeId.HasValue)
            {
                query = query.Where(c => c.CampaignTypeID == campaignTypeId.Value);
            }

            var isOrganization = !string.IsNullOrEmpty(createBy) && createBy.ToLower().Equals("organization");
            var isVolunteer = !string.IsNullOrEmpty(createBy) && createBy.ToLower().Equals("volunteer");
            // Apply createBy filter
            if (isOrganization)
            {
                query = query.Where(c => c.OrganizationID != null);
            }

            if (isVolunteer)
            {
                query = query.Where(c => c.OrganizationID == null);
            }

            if (!string.IsNullOrEmpty(campaignName))
                query = query.Where(c => c.Name != null && c.Name.ToLower().Contains(campaignName.Trim().ToLower()));

            // Apply pagination and sorting
            return query
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

        public IEnumerable<Campaign> GetCampaignsCreateByOM()
        {
            using var context = new VMODBContext();

            // Start with the base query
            var query = context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .Where(c => c.IsActive)
                .AsQueryable();
            return query.Where(c => c.OrganizationID != null).ToList();
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
