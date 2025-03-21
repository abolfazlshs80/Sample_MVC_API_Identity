using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SampleDomain.Database.Models;

namespace SampleAPI_APP.Database.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {


        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklist { get; set; }
        //public AppDbContext() : base() { }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        ////    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        //    base.OnModelCreating(modelBuilder);
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=DB_MVC_API_Sample_Identity;Integrated Security=True;Trust Server Certificate=True;Multiple Active Result Sets=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(AppDbContext)));
            base.OnModelCreating(modelBuilder);
        }



    }
}
