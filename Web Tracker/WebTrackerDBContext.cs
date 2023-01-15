using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using WebTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace WebTracker
{
    public class WebTrackerDBContext : DbContext
    {
        public WebTrackerDBContext() { }
       
        public WebTrackerDBContext(DbContextOptions<WebTrackerDBContext> options)
            : base(options) { 
        
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=WebTracker;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //    }
        //}
        public DbSet<WebTracker.Models.Action> Actions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Website> Websites { get; set; }
        public DbSet<Flow> Flows { get; set; }
        public DbSet<FlowData> FlowDatas { get; set; }
    }
}
