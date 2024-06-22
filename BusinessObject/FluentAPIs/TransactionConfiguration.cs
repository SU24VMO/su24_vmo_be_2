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
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transaction");
            builder.HasKey(x => x.TransactionID);
            builder.Property(x => x.CampaignID).IsRequired();
            builder.Property(x => x.TransactionType).IsRequired();
            builder.Property(x => x.Note).IsRequired();
            builder.Property(x => x.IsIncognito).IsRequired();
            builder.Property(x => x.PayerName).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.OrderId).IsRequired();
            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.TransactionStatus).IsRequired();

            builder.Property(x => x.OrderId)
                    .IsRequired()
                    .ValueGeneratedOnAdd();
        }
    }
}
