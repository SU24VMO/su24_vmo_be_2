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
    public class StatementPhaseConfiguration : IEntityTypeConfiguration<StatementPhase>
    {
        public void Configure(EntityTypeBuilder<StatementPhase> builder)
        {
            builder.ToTable("StatementPhase");
            builder.HasKey(x => x.StatementPhaseId);
            builder.Property(x => x.CampaignId).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsProcessing).IsRequired();
            builder.Property(x => x.IsEnd).IsRequired();

            builder.HasMany(x => x.StatementFiles)
                .WithOne(x => x.StatementPhase)
                .HasForeignKey(x => x.StatementPhaseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
