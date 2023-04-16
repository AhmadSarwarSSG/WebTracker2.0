using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using WebTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using WebTracker.Repositories;

namespace WebTracker.Models
{
    public class WebTrackerDBContext : IdentityDbContext<ApplicationUser>
    {
        public WebTrackerDBContext() {
            
        }

        public WebTrackerDBContext(DbContextOptions<WebTrackerDBContext> options)
            : base(options)
        {
            //try
            //{
            //    var databaseCreater = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            //    if (databaseCreater != null)
            //    {
            //        if (!databaseCreater.CanConnect())
            //        {
            //            databaseCreater.Create();
            //        }
            //        if (!databaseCreater.HasTables())
            //        {
            //            databaseCreater.CreateTables();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            Database.Migrate();
            //this.Database.ExecuteSqlRaw("CREATE VIEW [dbo].[SummedAction] AS SELECT AC.[FlowId], US.WebsiteId, CONCAT(AC.[Page],'[',STRING_AGG(CONCAT(AC.[Type],'(', AC.Content, ')'),'>'), ']') AS [Flowsummed] FROM [Actions] AS AC  Inner Join [Flows] AS FS ON AC.FlowId=FS.FlowId Inner Join [Users] AS US ON US.UserId=FS.UserId Group by US.WebsiteId, AC.FlowId, Ac.Page");
            //this.Database.ExecuteSqlRaw("CREATE VIEW [dbo].[ActionSummaryTable] AS SELECT [Flowsummed], [WebsiteId], COUNT([Flowsummed]) AS Count FROM [SummedAction] Group by Flowsummed, WebsiteId");
            //this.Database.ExecuteSqlRaw("CREATE VIEW [dbo].[SummedFlow] AS SELECT FD.[FlowId], US.WebsiteId, STRING_AGG(CONCAT(FD.[Page],''),'>') AS [Flowsummed] FROM [FlowDatas] AS FD Inner Join [Flows] AS FS ON FD.FlowId=FS.FlowId Inner Join [Users] AS US ON US.UserId=FS.UserId Group by FD.FlowId, US.WebsiteId");
            //this.Database.ExecuteSqlRaw("CREATE VIEW [dbo].[SummaryTable] AS SELECT [Flowsummed], [WebsiteId], COUNT([Flowsummed]) AS Count FROM [SummedFlow] Group by Flowsummed, WebsiteId");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.WebsiteUrl);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(@"Data Source=63.250.53.44;Initial Catalog=webtracker;User ID=ahmad;Password=ahmad@1234;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;");
               
            }
        }

        public DbSet<WebTracker.Models.Action> Actions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Website> Websites { get; set; }
        public DbSet<Flow> Flows { get; set; }
        public DbSet<FlowData> FlowDatas { get; set; }
    }
}
