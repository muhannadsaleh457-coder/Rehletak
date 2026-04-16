using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rehletak.Domain.Entites.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Presistense.Configurations.Auth
{
    public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasOne(u => u.user)
                .WithMany(r => r.refreshTokens)
                .HasForeignKey(u => u.userId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
