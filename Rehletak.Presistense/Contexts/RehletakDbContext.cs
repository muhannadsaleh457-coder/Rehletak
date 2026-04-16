using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rehletak.Domain.Entites.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Presistense.Contexts
{
    public class RehletakDbContext(DbContextOptions<RehletakDbContext> options) : IdentityDbContext<AppUser>(options)
    {
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

        }

        public DbSet<DriverProfile> driver_profiles { get; set; }
        public DbSet<RefreshToken> refresh_tokens { get; set; }
    }
}
