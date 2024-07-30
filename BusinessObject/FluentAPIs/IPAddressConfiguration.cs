using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.FluentAPIs
{
    public class IPAddressConfiguration : IEntityTypeConfiguration<IPAddress>
    {
        public void Configure(EntityTypeBuilder<IPAddress> builder)
        {
            builder.ToTable("IPAddress");
            builder.HasKey(x => x.IPAddressId);
            builder.Property(x => x.LoginTime).IsRequired();
            builder.Property(x => x.IPAddressValue).IsRequired();
            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
        }
    }
}
