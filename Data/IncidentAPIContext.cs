using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IncidentAPI.Models;

namespace IncidentAPI.Data
{
    public class IncidentAPIContext : DbContext
    {
        public IncidentAPIContext (DbContextOptions<IncidentAPIContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Incident>().Property(x => x.IncidentName).ValueGeneratedOnAdd();
        }
        public DbSet<IncidentAPI.Models.Incident> Incident { get; set; } = default!;
        public DbSet<IncidentAPI.Models.Account> Account { get; set; } = default!;
        public DbSet<IncidentAPI.Models.Contact> Contact { get; set; } = default!;
    }
}
