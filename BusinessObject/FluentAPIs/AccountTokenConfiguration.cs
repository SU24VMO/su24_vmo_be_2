using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace BusinessObject.FluentAPIs
{
    public class AccountTokenConfiguration : IEntityTypeConfiguration<AccountToken>
    {
        public void Configure(EntityTypeBuilder<AccountToken> builder)
        {
            builder.ToTable("AccountToken");
            builder.HasKey(x => x.AccountTokenId);
            builder.Property(x => x.AccountID).IsRequired();
            builder.Property(x => x.AccessToken).IsRequired();
            builder.Property(x => x.RefreshToken).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired();

        }
    }
}
