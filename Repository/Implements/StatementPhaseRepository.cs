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
    public class StatementPhaseRepository : IStatementPhaseRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StatementPhase> GetAll()
        {
            using var context = new VMODBContext();
            return context.StatementPhases
                .Include(a => a.Campaign).ToList();
        }

        public StatementPhase? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.StatementPhases
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.StatementPhaseId.Equals(id));
        }

        public StatementPhase? GetStatementPhaseByCampaignId(Guid campaignId)
        {
            using var context = new VMODBContext();
            return context.StatementPhases
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.CampaignId.Equals(campaignId));
        }

        public StatementPhase? Save(StatementPhase entity)
        {
            try
            {
                using var context = new VMODBContext();
                var statementPhaseAdded = context.StatementPhases.Add(entity);
                context.SaveChanges();
                return statementPhaseAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(StatementPhase entity)
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
