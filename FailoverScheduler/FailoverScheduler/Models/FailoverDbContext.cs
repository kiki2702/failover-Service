using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FailoverScheduler.Models
{
    public class FailoverDbContext : DbContext
    {
        public virtual DbSet<FailoverModel> FailoverMonitoring { get; set; }

        public FailoverDbContext(DbContextOptions<FailoverDbContext> options) : base(options) { }

       
    }
}
