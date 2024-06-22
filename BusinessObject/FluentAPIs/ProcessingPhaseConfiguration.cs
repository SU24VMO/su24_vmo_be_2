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
    public class ProcessingPhaseConfiguration : IEntityTypeConfiguration<ProcessingPhase>
    {
        public void Configure(EntityTypeBuilder<ProcessingPhase> builder)
        {
            builder.ToTable("ProcessingPhase");
            builder.HasKey(x => x.ProcessingPhaseId);
            builder.Property(x => x.CampaignId).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsEnd).IsRequired();
            builder.Property(x => x.IsProcessing).IsRequired();


            builder.HasMany(x => x.Activities)
                .WithOne(x => x.ProcessingPhase)
                .HasForeignKey(x => x.ProcessingPhaseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
