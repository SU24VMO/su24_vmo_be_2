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
    public class IPAddressRepository : IIPAddressRepository
    {
        public IEnumerable<IPAddress> GetAll()
        {
            using var context = new VMODBContext();
            return context.IPAddresses
                .Include(a => a.Account)
                .OrderByDescending(a => a.CreateDate).ToList();
        }

        public IPAddress? GetById(Guid id)
        {
            using var context = new VMODBContext();
            return context.IPAddresses
                .Include(a => a.Account)
                .OrderByDescending(a => a.CreateDate).FirstOrDefault(x => x.IPAddressId.Equals(id));
        }

        public IPAddress? Save(IPAddress entity)
        {
            try
            {
                using var context = new VMODBContext();
                var userAdded = context.IPAddresses.Add(entity);
                context.SaveChanges();
                return userAdded.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(IPAddress entity)
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
