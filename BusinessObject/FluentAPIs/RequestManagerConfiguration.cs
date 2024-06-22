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
    public class RequestManagerConfiguration : IEntityTypeConfiguration<RequestManager>
    {
        public void Configure(EntityTypeBuilder<RequestManager> builder)
        {
            builder.ToTable("RequestManager");
            builder.HasKey(x => x.RequestManagerID);
            builder.Property(x => x.AccountID).IsRequired();
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();

            builder.HasOne(x => x.Account);

            builder.HasMany(x => x.CreateCampaignRequests)
                .WithOne(x => x.RequestManager)
                .HasForeignKey(x => x.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.CreateOrganizationRequests)
                .WithOne(x => x.RequestManager)
                .HasForeignKey(x => x.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.CreateMemberRequests)
                .WithOne(x => x.RequestManager)
                .HasForeignKey(x => x.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.CreatePostRequests)
                .WithOne(x => x.RequestManager)
                .HasForeignKey(x => x.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CreateActivityRequests)
                .WithOne(x => x.RequestManager)
                .HasForeignKey(x => x.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CreateOrganizationManagerRequests)
                .WithOne(x => x.RequestManager)
                .HasForeignKey(x => x.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
