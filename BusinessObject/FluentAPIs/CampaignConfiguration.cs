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
    public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaign");
            builder.HasKey(x => x.CampaignID);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.CampaignTypeID).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Image).IsRequired();
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.ExpectedEndDate).IsRequired();
            builder.Property(x => x.TargetAmount).IsRequired();
            builder.Property(x => x.IsTransparent).IsRequired();
            builder.Property(x => x.CreateAt).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.IsModify).IsRequired();
            builder.Property(x => x.CanBeDonated).IsRequired();



            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.Campaign)
                .HasForeignKey(x => x.CampaignID)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.BankingAccount)
                .WithMany(x => x.Campaigns)
                .HasForeignKey(x => x.BankingAccountID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.ProcessingPhases)
                .WithOne(x=>x.Campaign)
                .HasForeignKey(x=> x.CampaignId)
                .OnDelete(DeleteBehavior.NoAction);


            // Ignore these navigation properties
            builder.Ignore(x => x.Organization);
            builder.Ignore(x => x.CampaignType);
        }
    }
}
