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
    public class CampaignTypeConfiguration : IEntityTypeConfiguration<CampaignType>
    {
        public void Configure(EntityTypeBuilder<CampaignType> builder)
        {
            builder.ToTable("CampaignType");
            builder.HasKey(x => x.CampaignTypeID);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.CreateAt).IsRequired();
            builder.Property(x => x.IsValid).IsRequired();

            builder.HasMany(x => x.Campaigns)
                .WithOne(x => x.CampaignType)
                .HasForeignKey(x => x.CampaignTypeID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
