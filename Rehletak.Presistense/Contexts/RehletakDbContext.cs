using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Presistense.Contexts
{
    public class RehletakDbContext(DbContextOptions<RehletakDbContext> options) : IdentityDbContext(options)
    {
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
