using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectHero.DomainModal;

namespace ProjectHero.Data
{
    public class ProjectHeroContext : IdentityDbContext<HeroUser> //Previous it was DbContext
                                                        //Now changed to IdentityDbContext, due to we are using Identity now for authentication purpose
    {
        public ProjectHeroContext (DbContextOptions<ProjectHeroContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<HeroDM> Hero { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=HeroDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            //optionsBuilder.UseSqlServer("Server=192.168.29.71;Database=HeroAPIDB;User Id=sa;Password=123@Reno;MultipleActiveResultSets=true;Encrypt=False;");
            base.OnConfiguring(optionsBuilder);
            //For deployment we use below method for database creation
            /*optionsBuilder.UseSqlServer("Server=192.168.29.71;Database=HeroDB;User Id=sa;Password=123@Reno;MultipleActiveResultSets=true;Encrypt=False;");
            base.OnConfiguring(optionsBuilder);*/
        }
    }
}
