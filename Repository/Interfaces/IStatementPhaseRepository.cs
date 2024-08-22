using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IStatementPhaseRepository : ICrudBaseRepository<StatementPhase, Guid>
    {
        public StatementPhase? GetStatementPhaseByCampaignId(Guid campaignId);
        public Task<StatementPhase?> GetByIdAsync(Guid id);


    }
}
