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
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");
            builder.HasKey(x => x.AccountID);
            builder.Property(x => x.Email).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SaltPassword).IsRequired();
            builder.Property(x => x.HashPassword).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.Role).IsRequired();
            builder.HasIndex(x => x.Username).IsUnique();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsBlocked).IsRequired();
            builder.Property(x => x.IsActived).IsRequired();

            builder.HasMany(x => x.Notifications)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.AccountTokens)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.BankingAccounts)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
