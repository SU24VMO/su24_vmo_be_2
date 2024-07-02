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
    public class CampaignTypeRepository : ICampaignTypeRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CampaignType> GetAll()
        {
            using var context = new VMODBContext();
            return context.CampaignTypes
                .Include(a => a.Campaigns)
                .OrderByDescending(a => a.CreateAt).ToList();
        }

        public CampaignType? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.CampaignTypes
                .Include(a => a.Campaigns).ToList()
                .FirstOrDefault(d => d.CampaignTypeID.Equals(id));
        }

        public CampaignType? Save(CampaignType entity)
        {
            try
            {
                using var context = new VMODBContext();
                var campaignTypeCreated = context.CampaignTypes.Add(entity);
                context.SaveChanges();
                return campaignTypeCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(CampaignType entity)
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
