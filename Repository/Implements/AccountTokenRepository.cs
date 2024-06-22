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
    public class AccountTokenRepository : IAccountTokenRepository
    {
        public AccountToken? CheckRefreshToken(string code)
        {
            using var context = new VMODBContext();
            var accountToken = context.AccountTokens.FirstOrDefault(x => x.CodeRefreshToken.Equals(code));
            return accountToken;
        }

        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountToken> GetAll()
        {
            using var context = new VMODBContext();
            return context.AccountTokens
                .Include(a => a.Account).ToList();
        }

        public AccountToken? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.AccountTokens
                .Include(a => a.Account).ToList().FirstOrDefault(a => a.AccountTokenId.Equals(id));
        }

        public AccountToken? Save(AccountToken entity)
        {
            try
            {
                using var context = new VMODBContext();
                var userAdded = context.AccountTokens.Add(entity);
                context.SaveChanges();
                return userAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(AccountToken entity)
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
