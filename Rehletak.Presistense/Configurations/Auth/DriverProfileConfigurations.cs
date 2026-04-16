using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rehletak.Domain.Entites.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Presistense.Configurations.Auth
{
    public class DriverProfileConfigurations : IEntityTypeConfiguration<DriverProfile>
    {
        public void Configure(EntityTypeBuilder<DriverProfile> builder)
        {
            builder.HasOne(u => u.user)
                .WithOne()
                .HasForeignKey<DriverProfile>(u => u.user_id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
