using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.FluentAPIs
{
    public class BankingAccountConfiguration : IEntityTypeConfiguration<BankingAccount>
    {
        public void Configure(EntityTypeBuilder<BankingAccount> builder)
        {
            builder.ToTable("BankingAccount");
            builder.HasKey(x => x.BankingAccountID);
            builder.Property(x => x.BankingName).IsRequired();
            builder.Property(x => x.AccountNumber).IsRequired();
            builder.Property(x => x.AccountName).IsRequired();
            builder.Property(x => x.QRCode).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.IsAvailable).IsRequired();


            builder.HasMany(x => x.Transactions)
               .WithOne(x => x.BankingAccount)
               .HasForeignKey(x => x.BankingAccountID)
               .OnDelete(DeleteBehavior.NoAction);

            
        }
    }
}
