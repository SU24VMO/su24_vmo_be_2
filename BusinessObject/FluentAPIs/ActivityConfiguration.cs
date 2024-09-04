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
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activity");
            builder.HasKey(x => x.ActivityId);
            builder.Property(x => x.ProcessingPhaseId).IsRequired();
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            builder.HasMany(x => x.ActivityImages)
                .WithOne(x => x.Activity)
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.ActivityStatementFiles)
                .WithOne(x => x.Activity)
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
