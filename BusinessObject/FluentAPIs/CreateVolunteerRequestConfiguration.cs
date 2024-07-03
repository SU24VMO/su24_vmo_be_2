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
    public class CreateVolunteerRequestConfiguration : IEntityTypeConfiguration<CreateVolunteerRequest>
    {
        public void Configure(EntityTypeBuilder<CreateVolunteerRequest> builder)
        {
            builder.ToTable("CreateVolunteerRequest");
            builder.HasKey(x => x.CreateVolunteerRequestID);
            builder.Property(x => x.MemberID).IsRequired();
            builder.Property(x => x.CreateBy).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.IsRejected).IsRequired();
            builder.Property(x => x.IsPending).IsRequired();

        }
    }
}
