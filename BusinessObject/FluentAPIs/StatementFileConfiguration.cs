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
    public class StatementFileConfiguration : IEntityTypeConfiguration<StatementFile>
    {
        public void Configure(EntityTypeBuilder<StatementFile> builder)
        {
            builder.ToTable("StatementFile");
            builder.HasKey(x => x.StatementFileId);
            builder.Property(x => x.StatementPhaseId).IsRequired();
            builder.Property(x => x.Link).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
        }
    }
}
