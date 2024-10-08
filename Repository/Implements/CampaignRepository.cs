﻿using BusinessObject.Enums;
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

        public IEnumerable<Campaign> GetCampaignsCreateByVolunteer(string? phase)
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
                .AsQueryable();
            if (phase.Trim().ToLower().Equals("donate-phase"))
            {
                query = query.Where(c =>
                    c.DonatePhase != null && c.IsActive && c.DonatePhase.IsProcessing == true);
            }

            if (phase.Trim().ToLower().Equals("processing-phase"))
            {
                query = query.Where(c =>
                    c.ProcessingPhases != null && c.IsActive && c.ProcessingPhases.Any(pp => pp.IsProcessing) == true);
            }

            if (phase.Trim().ToLower().Equals("statement-phase"))
            {
                query = query.Where(c =>
                    c.StatementPhase != null && c.IsActive && c.StatementPhase.IsProcessing == true);
            }
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

        public IEnumerable<Campaign> GetAllCampaignsTierIWithActiveStatus(string? campaignName)
        {
            using var context = new VMODBContext();
            var query = context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .OrderByDescending(a => a.CreateAt).ToList().Where(c => c.Name != null && c.IsActive == true && c.CampaignTier == CampaignTier.FullDisbursementCampaign && c.Name.ToLower().Trim().Contains(campaignName?.ToLower().Trim() ?? string.Empty));
            return query.ToList();
        }

        public IEnumerable<Campaign> GetAllCampaignsTierIWithActiveStatus(string? campaignName, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            var query = context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .OrderByDescending(a => a.CreateAt).ToList().Where(c => c.Name != null && c.IsActive == true && c.CampaignTier == CampaignTier.FullDisbursementCampaign && c.Name.ToLower().Trim().Contains(campaignName?.ToLower().Trim() ?? string.Empty));
            int totalCount = query.Count();

            // Set pageSize to the total count if it's not provided
            int size = pageSize ?? totalCount;
            int page = pageNo ?? 1;

            // Apply pagination
            return query
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }

        public IEnumerable<Campaign> GetAllCampaignsTierIIWithActiveStatus(string? campaignName)
        {
            using var context = new VMODBContext();
            var query = context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .OrderByDescending(a => a.CreateAt).ToList().Where(c => c.Name != null && c.IsActive == true && c.CampaignTier == CampaignTier.PartialDisbursementCampaign && c.Name.ToLower().Trim().Contains(campaignName?.ToLower().Trim() ?? string.Empty));
            return query.ToList();
        }

        public IEnumerable<Campaign> GetAllCampaignsTierIIWithActiveStatus(string? campaignName, int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            var query = context.Campaigns
                .Include(a => a.Organization)
                .Include(a => a.CampaignType)
                .Include(a => a.Transactions)
                .Include(a => a.ProcessingPhases)
                .Include(a => a.DonatePhase)
                .Include(a => a.StatementPhase)
                .OrderByDescending(a => a.CreateAt).ToList().Where(c => c.Name != null && c.IsActive == true && c.CampaignTier == CampaignTier.PartialDisbursementCampaign && c.Name.ToLower().Trim().Contains(campaignName?.ToLower().Trim() ?? string.Empty));
            int totalCount = query.Count();

            // Set pageSize to the total count if it's not provided
            int size = pageSize ?? totalCount;
            int page = pageNo ?? 1;

            // Apply pagination
            return query
                .Skip((page - 1) * size)
                .Take(size)
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

        public IEnumerable<Campaign> GetCampaignsCreateByOM(string? phase)
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
                .Where(c => c.ProcessingPhases != null && c.IsActive && c.ProcessingPhases.Any(pp => pp.IsProcessing) == true)
                .AsQueryable();

            if (phase.Trim().ToLower().Equals("donate-phase"))
            {
                query = query.Where(c =>
                    c.DonatePhase != null && c.IsActive && c.DonatePhase.IsProcessing == true);
            }

            if (phase.Trim().ToLower().Equals("processing-phase"))
            {
                query = query.Where(c =>
                    c.ProcessingPhases != null && c.IsActive && c.ProcessingPhases.Any(pp => pp.IsProcessing) == true);
            }

            if (phase.Trim().ToLower().Equals("statement-phase"))
            {
                query = query.Where(c =>
                    c.StatementPhase != null && c.IsActive && c.StatementPhase.IsProcessing == true);
            }
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
