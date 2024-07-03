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
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("Member");
            builder.HasKey(x => x.MemberID);
            builder.Property(x => x.AccountID).IsRequired();
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();
            builder.Property(x => x.IsVerified).IsRequired();

            builder.HasOne(x => x.Account);
            builder.HasMany(x => x.CreateCampaignRequests)
                .WithOne(x => x.Member)
                .HasForeignKey(x => x.CreateByMember)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.CreateMemberVerifiedRequests)
                .WithOne(x => x.Member)
                .HasForeignKey(x => x.CreateBy)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.CreatePostRequests)
                .WithOne(x => x.Member)
                .HasForeignKey(x => x.CreateByMember)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
