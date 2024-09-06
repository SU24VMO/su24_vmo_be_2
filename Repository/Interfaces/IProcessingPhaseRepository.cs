﻿using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IProcessingPhaseRepository : ICrudBaseRepository<ProcessingPhase, Guid>
    {
        public List<ProcessingPhase> GetProcessingPhaseByCampaignId(Guid campaignId);
        public IEnumerable<ProcessingPhase> GetAll(int? pageSize, int? pageNo);

    }
}
