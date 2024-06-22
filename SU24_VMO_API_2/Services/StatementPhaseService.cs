using BusinessObject.Models;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class StatementPhaseService
    {
        private readonly IStatementPhaseRepository _repository;
        private readonly ICampaignRepository _campaignRepository;

        public StatementPhaseService(IStatementPhaseRepository repository, ICampaignRepository campaignRepository)
        {
            _repository = repository;
            _campaignRepository = campaignRepository;
        }

        public IEnumerable<StatementPhase> GetAll()
        {
            return _repository.GetAll();
        }

        public StatementPhase? GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public StatementPhase? CreateStatementPhase(CreateStatementPhaseRequest request)
        {
            if(_campaignRepository.GetById(request.CampaignId) == null)
            {
                return null;
            }
            var statementPhase = new StatementPhase
            {
                StatementPhaseId = Guid.NewGuid(),
                CampaignId = request.CampaignId,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsEnd = request.IsEnd,
                IsProcessing = request.IsProcessing,
            };

            return _repository.Save(statementPhase);
        }

        public void UpdateStatementPhase(UpdateStatementPhaseRequest request)
        {
            var statementPhase = _repository.GetById(request.StatementPhaseId);
            if (statementPhase == null)
            {
                return;
            }
            if (!String.IsNullOrEmpty(request.Name))
            {
                statementPhase.Name = request.Name;
            }
            if (!String.IsNullOrEmpty(request.StartDate.ToString()))
            {
                statementPhase.StartDate = request.StartDate;
            }
            if (!String.IsNullOrEmpty(request.EndDate.ToString()))
            {
                statementPhase.EndDate = request.EndDate;
            }
            if (request.IsProcessing != null)
            {
                statementPhase.IsProcessing = (bool)request.IsProcessing;
            }
            if (request.IsEnd != null)
            {
                statementPhase.IsEnd = (bool)request.IsEnd;
            }
            _repository.Update(statementPhase);
        }
    }
}
