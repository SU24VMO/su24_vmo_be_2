using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Enums;

namespace Repository.Implements
{
    public class ProcessingPhaseRepository : IProcessingPhaseRepository
    {
        public void DeleteById(Guid id)
        {
            using var context = new VMODBContext();
            var processingPhase = context.ProcessingPhases.FirstOrDefault(ac => ac.ProcessingPhaseId.Equals(id));
            if (processingPhase != null)
            {
                context.ProcessingPhases.Remove(processingPhase);
                context.SaveChanges();
            }
        }

        public IEnumerable<ProcessingPhase> GetAll()
        {
            using var context = new VMODBContext();
            return context.ProcessingPhases
                .Include(a => a.Campaign)
                .Include(a => a.ProcessingPhaseStatementFiles)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public ProcessingPhase? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.ProcessingPhases
                .Include(a => a.Campaign)
                .Include(a => a.ProcessingPhaseStatementFiles).ToList()
                .FirstOrDefault(d => d.ProcessingPhaseId.Equals(id));
        }

        public List<ProcessingPhase> GetProcessingPhaseByCampaignId(Guid campaignId)
        {
            using var context = new VMODBContext();
            return context.ProcessingPhases
                .Include(a => a.Campaign)
                .Include(a => a.ProcessingPhaseStatementFiles)
                .Where(d => d.CampaignId.Equals(campaignId)).ToList();
        }

        public IEnumerable<ProcessingPhase> GetAll(int? pageSize, int? pageNo)
        {
            using var context = new VMODBContext();
            var query = context.ProcessingPhases
                .Include(a => a.Campaign)
                .Include(a => a.ProcessingPhaseStatementFiles)
                .OrderByDescending(a => a.CreateDate).ToList()
                .Where(p => p.Campaign is { CampaignTier: CampaignTier.PartialDisbursementCampaign });
            int totalCount = query.Count();

            // Set pageSize to the total count if it's not provided
            int size = pageSize ?? 10;
            int page = pageNo ?? 1;

            // Apply pagination
            return query
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }

        public ProcessingPhase? Save(ProcessingPhase entity)
        {
            try
            {
                using var context = new VMODBContext();
                var processingPhaseAdded = context.ProcessingPhases.Add(entity);
                context.SaveChanges();
                return processingPhaseAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(ProcessingPhase entity)
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
