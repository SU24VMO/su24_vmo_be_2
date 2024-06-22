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
    public class CreateOrganizationRequestConfiguration : IEntityTypeConfiguration<CreateOrganizationRequest>
    {
        public void Configure(EntityTypeBuilder<CreateOrganizationRequest> builder)
        {
            builder.ToTable("CreateOrganizationRequest");
            builder.HasKey(x => x.CreateOrganizationRequestID);
            builder.Property(x => x.OrganizationID).IsRequired();
            builder.Property(x => x.CreateBy).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.IsRejected).IsRequired();
            builder.Property(x => x.IsPending).IsRequired();

        }
    }
}
