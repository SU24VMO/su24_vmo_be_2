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
            throw new NotImplementedException();
        }

        public IEnumerable<ProcessingPhase> GetAll()
        {
            using var context = new VMODBContext();
                return context.ProcessingPhases
                    .Include(a => a.Campaign).OrderByDescending(a => a.CreateDate).ToList();
        }

        public ProcessingPhase? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.ProcessingPhases
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.ProcessingPhaseId.Equals(id));
        }

        public ProcessingPhase? GetProcessingPhaseByCampaignId(Guid campaignId)
        {
            using var context = new VMODBContext();
            return context.ProcessingPhases
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.CampaignId.Equals(campaignId));
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
