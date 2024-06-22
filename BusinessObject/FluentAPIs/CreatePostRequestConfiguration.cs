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
    public class CreatePostRequestConfiguration : IEntityTypeConfiguration<CreatePostRequest>
    {
        public void Configure(EntityTypeBuilder<CreatePostRequest> builder)
        {
            builder.ToTable("CreatePostRequest");
            builder.HasKey(x => x.CreatePostRequestID);
            builder.Property(x => x.PostID).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.IsRejected).IsRequired();
            builder.Property(x => x.IsPending).IsRequired();
        }
    }
}
