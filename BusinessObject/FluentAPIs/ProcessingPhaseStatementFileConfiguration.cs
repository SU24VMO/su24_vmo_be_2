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
    public class ProcessingPhaseStatementFileConfiguration : IEntityTypeConfiguration<ProcessingPhaseStatementFile>
    {
        public void Configure(EntityTypeBuilder<ProcessingPhaseStatementFile> builder)
        {
            builder.ToTable("ProcessingPhaseStatementFile");
            builder.HasKey(x => x.ProcessingPhaseStatementFileId);
            builder.Property(x => x.ProcessingPhaseId).IsRequired();
            builder.Property(x => x.Link).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
        }
    }
}
