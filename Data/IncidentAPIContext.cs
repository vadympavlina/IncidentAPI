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

        public DbSet<IncidentAPI.Models.Incident> Incident { get; set; } = default!;
        public DbSet<IncidentAPI.Models.Account> Account { get; set; } = default!;
        public DbSet<IncidentAPI.Models.Contact> Contact { get; set; } = default!;
    }
}
