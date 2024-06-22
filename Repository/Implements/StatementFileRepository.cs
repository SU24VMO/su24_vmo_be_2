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
    public class StatementFileRepository : IStatementFileRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StatementFile> GetAll()
        {
            using var context = new VMODBContext();
            return context.StatementFiles
                .Include(a => a.StatementPhase).ToList();
        }

        public StatementFile? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.StatementFiles
                .Include(a => a.StatementPhase).ToList()
                .FirstOrDefault(a => a.StatementPhaseId.Equals(id));
        }

        public StatementFile? Save(StatementFile entity)
        {
            try
            {
                using var context = new VMODBContext();
                var statementFileAdded = context.StatementFiles.Add(entity);
                context.SaveChanges();
                return statementFileAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(StatementFile entity)
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
