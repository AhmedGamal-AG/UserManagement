
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullCoreProject.Models
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            
            //foreach(var foriegnKey in modelBuilder.Model.GetEntityTypes().SelectMany(e =>e.GetForeignKeys()))
            //{
            //    foriegnKey.DeleteBehavior = DeleteBehavior.Restrict;
            //}
        }
        public DbSet<Employee> Employees { get; set; }

    }
}
