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
    public class ActivityImageConfiguration : IEntityTypeConfiguration<ActivityImage>
    {
        public void Configure(EntityTypeBuilder<ActivityImage> builder)
        {
            builder.ToTable("ActivityImage");
            builder.HasKey(x => x.ActivityImageId);
            builder.Property(x => x.ActivityId).IsRequired();
            builder.Property(x => x.Link).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

        }
    }
}
