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
    public class OrganizationManagerConfiguration : IEntityTypeConfiguration<OrganizationManager>
    {
        public void Configure(EntityTypeBuilder<OrganizationManager> builder)
        {
            builder.ToTable("OrganizationManager");
            builder.HasKey(x => x.OrganizationManagerID);
            builder.Property(x => x.AccountID).IsRequired();
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();

            builder.HasOne(x => x.Account);
            builder.HasMany(x => x.CreateCampaignRequests)
                .WithOne(x => x.OrganizationManager)
                .HasForeignKey(x => x.CreateByOM)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasMany(x => x.Organizations)
                .WithOne(x => x.OrganizationManager)
                .HasForeignKey(x => x.OrganizationManagerID)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasMany(x => x.CreateOrganizationRequests)
                .WithOne(x => x.OrganizationManager)
                .HasForeignKey(x => x.CreateBy)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.CreatePostRequests)
                .WithOne(x => x.OrganizationManager)
                .HasForeignKey(x => x.CreateByOM)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
