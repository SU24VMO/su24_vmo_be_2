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
