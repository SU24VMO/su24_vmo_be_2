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
    public class DonatePhaseConfiguration : IEntityTypeConfiguration<DonatePhase>
    {
        public void Configure(EntityTypeBuilder<DonatePhase> builder)
        {
            builder.ToTable("DonatePhase");
            builder.HasKey(x => x.DonatePhaseId);
            builder.Property(x => x.CampaignId).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.CurrentMoney).IsRequired();
            builder.Property(x => x.Percent).IsRequired();
            builder.Property(x => x.IsProcessing).IsRequired();
            builder.Property(x => x.IsEnd).IsRequired();

        }
    }
}
