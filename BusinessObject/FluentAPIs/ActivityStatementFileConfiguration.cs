using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.FluentAPIs
{
    public class ActivityStatementFileConfiguration : IEntityTypeConfiguration<ActivityStatementFile>
    {
        public void Configure(EntityTypeBuilder<ActivityStatementFile> builder)
        {
            builder.ToTable("ActivityStatementFile");
            builder.HasKey(x => x.ActivityStatementFileId);
            builder.Property(x => x.ActivityId).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Link).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

        }
    }
}
