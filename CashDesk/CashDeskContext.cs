using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk
{
    class CashDeskContext: DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Membership> Memberships { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("database");
        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>()
            .HasIndex(m => m.LastName);
        modelBuilder.Entity<Membership>().HasIndex(m => m.MemberId);
    }
    }
}
