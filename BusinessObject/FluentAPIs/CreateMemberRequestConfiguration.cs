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
    public class CreateMemberRequestConfiguration : IEntityTypeConfiguration<CreateMemberRequest>
    {
        public void Configure(EntityTypeBuilder<CreateMemberRequest> builder)
        {
            builder.ToTable("CreateMemberRequest");
            builder.HasKey(x => x.CreateMemberRequestID);
            builder.Property(x => x.UserID).IsRequired();
            builder.Property(x => x.CreateBy).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.IsRejected).IsRequired();
            builder.Property(x => x.IsPending).IsRequired();

        }
    }
}
