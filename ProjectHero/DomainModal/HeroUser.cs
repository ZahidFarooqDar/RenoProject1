using Microsoft.AspNetCore.Identity;

namespace ProjectHero.DomainModal
{
    /*Here we have defined our UserClass if not then in DbContextClass we should not have to give explicit name at extended class
     public class ProjectHeroContext : IdentityDbContext<HeroUser> , The heroUser here is defined UserClass for having properties viz FirstName & LastName
     Also we do not have do explicitly do this in Program.cs
    builder.Services.AddIdentity<HeroUser, IdentityRole>() , Here HeroUser is defined UserClass-> can be replaced by UserClass if HeroUser were'nt defined
    .AddEntityFrameworkStores<ProjectHeroContext>()    
    .AddDefaultTokenProviders();
     */
    public class HeroUser : IdentityUser //Extending IdentityUser allows to create tables
                                         //Here some columns are predefined but we can declare our own properties as well
    {
        public string? FirstName { get; set; }   //Here we have defined our own properties 
        public string? LastName { get; set; }
    }
}
