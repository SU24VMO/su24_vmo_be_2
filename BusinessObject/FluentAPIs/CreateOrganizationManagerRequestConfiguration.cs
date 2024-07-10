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
    public class CreateOrganizationManagerRequestConfiguration : IEntityTypeConfiguration<CreateOrganizationManagerRequest>
    {
        public void Configure(EntityTypeBuilder<CreateOrganizationManagerRequest> builder)
        {
            builder.ToTable("CreateOrganizationManagerRequest");
            builder.HasKey(x => x.CreateOrganizationManagerRequestID);
            builder.Property(x => x.OrganizationManagerID).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.PhoneNumber).IsRequired();
            builder.Property(x => x.CreateBy).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.IsRejected).IsRequired();
            builder.Property(x => x.IsPending).IsRequired();
        }
    }
}
