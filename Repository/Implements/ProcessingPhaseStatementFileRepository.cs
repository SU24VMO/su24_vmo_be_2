using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implements
{
    public class ProcessingPhaseStatementFileRepository : IProcessingPhaseStatementFileRepository
    {
        public IEnumerable<ProcessingPhaseStatementFile> GetAll()
        {
            using var context = new VMODBContext();
            return context.ProcessingPhaseStatementFiles
                .Include(a => a.ProcessingPhase)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public ProcessingPhaseStatementFile? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.ProcessingPhaseStatementFiles
                .Include(a => a.ProcessingPhase).ToList()
                .FirstOrDefault(a => a.ProcessingPhaseStatementFileId.Equals(id));
        }

        public ProcessingPhaseStatementFile? Save(ProcessingPhaseStatementFile entity)
        {
            try
            {
                using var context = new VMODBContext();
                var processingPhaseStatementFileCreated = context.ProcessingPhaseStatementFiles.Add(entity);
                context.SaveChanges();
                return processingPhaseStatementFileCreated.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void DeleteById(Guid id)
        {
            using var context = new VMODBContext();
            var processingPhaseStatementFile = context.ProcessingPhaseStatementFiles.FirstOrDefault(ac => ac.ProcessingPhaseStatementFileId.Equals(id));
            if (processingPhaseStatementFile != null)
            {
                context.ProcessingPhaseStatementFiles.Remove(processingPhaseStatementFile);
                context.SaveChanges();
            }
        }

        public void Update(ProcessingPhaseStatementFile entity)
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

        public IEnumerable<ProcessingPhaseStatementFile> GetProcessingPhaseStatementFilesByProcessingPhaseId(Guid processingPhaseId)
        {
            using var context = new VMODBContext();
            return context.ProcessingPhaseStatementFiles
                .Include(a => a.ProcessingPhase)
                .Where(p => p.ProcessingPhaseId.Equals(processingPhaseId))
                .OrderByDescending(a => a.CreateDate).ToList();
        }
    }
}
