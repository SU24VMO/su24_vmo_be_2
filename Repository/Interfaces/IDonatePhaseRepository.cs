using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IDonatePhaseRepository : ICrudBaseRepository<DonatePhase, Guid>
    {
        public DonatePhase? GetDonatePhaseByCampaignId(Guid campaignId);
    }
}
