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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(x => x.UserID);
            builder.Property(x => x.AccountID).IsRequired();
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();
            builder.Property(x => x.IsVerified).IsRequired();

            builder.HasOne(x => x.Account);
            builder.HasMany(x => x.CreateCampaignRequests)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.CreateByUser)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.CreateUserVerifiedRequests)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.CreateBy)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.CreatePostRequests)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.CreateByUser)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
