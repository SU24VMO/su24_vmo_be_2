
using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;
using System.Text.RegularExpressions;

namespace SU24_VMO_API.Services
{
    public class StatementFileService
    {
        private readonly IStatementFileRepository _statementFileRepository;
        private readonly IStatementPhaseRepository _statementPhaseRepository;
        private readonly FirebaseService _firebaseService;

        public StatementFileService(IStatementFileRepository statementFileRepository, IStatementPhaseRepository statementPhaseRepository, 
            FirebaseService firebaseService)
        {
            _statementFileRepository = statementFileRepository;
            _statementPhaseRepository = statementPhaseRepository;
            _firebaseService = firebaseService;
        }

        public IEnumerable<StatementFile> GetAll()
        {
            return _statementFileRepository.GetAll();
        }

        public StatementFile? GetById(Guid id)
        {
            return _statementFileRepository.GetById(id);
        }

        public async Task<StatementFile?> UploadStatementFileAsync(CreateStatementFileRequest request)
        {
            var statementPhase = _statementPhaseRepository.GetById(request.StatementPhaseId);
            if (statementPhase == null) return null;
            var statementFile = new StatementFile
            {
                StatementFileId = Guid.NewGuid(),
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                StatementPhaseId = statementPhase.StatementPhaseId,
                Link = await _firebaseService.UploadImage(request.StatementFile)
            };
            return _statementFileRepository.Save(statementFile);
        }

        public void Update(StatementFile entity)
        {
            _statementFileRepository.Update(entity);
        }
    }
}
