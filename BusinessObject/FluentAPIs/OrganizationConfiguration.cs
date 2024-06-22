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
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("Organization");
            builder.HasKey(x => x.OrganizationID);
            builder.Property(x => x.OrganizationManagerID).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Logo).IsRequired();
            builder.Property(x => x.FoundingDate).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.IsModify).IsRequired();

            builder.HasMany(x => x.Achievements)
                .WithOne(x => x.Organization)
                .HasForeignKey(x => x.OrganizationID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Campaigns)
                .WithOne(x => x.Organization)
                .HasForeignKey(x => x.OrganizationID)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Ignore(x => x.OrganizationManager);

        }
    }
}
