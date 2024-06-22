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
    public class CreateCampaignRequestConfiguration : IEntityTypeConfiguration<CreateCampaignRequest>
    {
        public void Configure(EntityTypeBuilder<CreateCampaignRequest> builder)
        {
            builder.ToTable("CreateCampaignRequest");
            builder.HasKey(x => x.CreateCampaignRequestID);
            builder.Property(x => x.CampaignID).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.IsRejected).IsRequired();
            builder.Property(x => x.IsPending).IsRequired();
        }
    }
}
