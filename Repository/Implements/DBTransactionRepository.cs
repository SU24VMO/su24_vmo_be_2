using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class DBTransactionRepository : IDBTransactionRepository
    {
        private readonly VMODBContext _context;
        private IDbContextTransaction? _transaction;

        public DBTransactionRepository()
        {
        }

        public async Task CommitAsync()
        {
            if (_transaction != null) await _transaction.CommitAsync();
        }

        public async Task CreateTransactionAsync()
        {
            var _context = new VMODBContext(); 
            if (_transaction != null) _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public bool IsExist()
        {
            return _transaction != null;
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null) await _transaction.RollbackAsync();
        }
    }
}
