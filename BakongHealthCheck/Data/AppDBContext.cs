
using BakongHealthCheck.Entities;
using Microsoft.EntityFrameworkCore;

namespace BakongHealthCheck.Data
{
    public class AppDBContext : DbContext 
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        { }

        public DbSet<MBService> mbService { get; set; }
      
    }
}
