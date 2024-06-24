﻿
using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using System.Text.RegularExpressions;

namespace SU24_VMO_API.Services
{
    public class StatementFileService
    {
        private readonly IStatementFileRepository _statementFileRepository;
        private readonly IStatementPhaseRepository _statementPhaseRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly FirebaseService _firebaseService;

        public StatementFileService(IStatementFileRepository statementFileRepository, IStatementPhaseRepository statementPhaseRepository,
            FirebaseService firebaseService, IAccountRepository accountRepository, INotificationRepository notificationRepository)
        {
            _statementFileRepository = statementFileRepository;
            _statementPhaseRepository = statementPhaseRepository;
            _firebaseService = firebaseService;
            _accountRepository = accountRepository;
            _notificationRepository = notificationRepository;
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
            if (statementPhase == null) throw new NotFoundException("Statement Phase not found!");
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) throw new NotFoundException("Account not found!");

            var statementFile = new StatementFile
            {
                StatementFileId = Guid.NewGuid(),
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                StatementPhaseId = statementPhase.StatementPhaseId,
                CreateBy = account.AccountID,
                Link = await _firebaseService.UploadImage(request.StatementFile)
            };
            var statementFileCreated = _statementFileRepository.Save(statementFile);
            if(statementFileCreated != null)
            {
                var notification = new Notification
                {
                    NotificationID = Guid.NewGuid(),
                    NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    IsSeen = false,
                    AccountID = account.AccountID,
                    Content = "Bạn vừa tải lên thành công bản sao kê!"
                };
                _notificationRepository.Save(notification);
            }
            return statementFileCreated;
        }

        public void Update(StatementFile entity)
        {
            _statementFileRepository.Update(entity);
        }
    }
}
