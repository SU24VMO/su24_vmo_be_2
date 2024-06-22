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
    public class DonatePhaseRepository : IDonatePhaseRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DonatePhase> GetAll()
        {
            using var context = new VMODBContext();
            return context.DonatePhases
                .Include(a => a.Campaign).ToList();
        }

        public DonatePhase? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.DonatePhases
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.DonatePhaseId.Equals(id));
        }

        public DonatePhase? GetDonatePhaseByCampaignId(Guid campaignId)
        {
            using var context = new VMODBContext();
            return context.DonatePhases
                .Include(a => a.Campaign).ToList()
                .FirstOrDefault(d => d.CampaignId.Equals(campaignId));
        }

        public DonatePhase? Save(DonatePhase entity)
        {
            try
            {
                using var context = new VMODBContext();
                var donatePhaseAdded = context.DonatePhases.Add(entity);
                context.SaveChanges();
                return donatePhaseAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(DonatePhase entity)
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
