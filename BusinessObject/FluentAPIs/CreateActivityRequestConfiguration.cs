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
    public class CreateActivityRequestConfiguration : IEntityTypeConfiguration<CreateActivityRequest>
    {
        public void Configure(EntityTypeBuilder<CreateActivityRequest> builder)
        {
            builder.ToTable("CreateActivityRequest");
            builder.HasKey(x => x.CreateActivityRequestID);
            builder.Property(x => x.ActivityID).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.IsRejected).IsRequired();
            builder.Property(x => x.IsPending).IsRequired();
        }
    }
}
