using BusinessObject.Models;
using Repository.Interfaces;

namespace SU24_VMO_API.Services
{
    public class DBTransactionService
    {
        private readonly IDBTransactionRepository _transactionRepository;

        public DBTransactionService(IDBTransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task CommitAsync()
        {
            await _transactionRepository.CommitAsync();
        }

        public async Task CreateTransactionAsync()
        {
            await _transactionRepository.CreateTransactionAsync();
        }

        public async Task DisposeAsync()
        {
            await _transactionRepository?.DisposeAsync();
        }

        public bool IsExist()
        {
           return _transactionRepository.IsExist();
        }

        public async Task Rollback()
        {
            await _transactionRepository.RollbackAsync();
        }
    }
}
