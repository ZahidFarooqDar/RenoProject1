using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using ProjectHero.Data;
using ProjectHero.DomainModal;
using ProjectHero.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
//builder.Services.AddSingleton<IHeroRepository, HeroRepository>();
/* Since ProjectHeroContext is a scoped service, you should also register your IHeroRepository as a scoped service to ensure that the lifetimes match. 
 * Replace the AddSingleton line with AddScoped:*/


//builder.Services.AddScoped<IHeroRepository, HeroRepository>(); //We need to manage our dependencies here   
builder.Services.TryAddScoped<IHeroRepository, HeroRepository>();//Will be used if this service is not used before then it will use this one also
                                                                 //if already present then it will skip
                                                                 //Interface must be different then it will work otherwise skip
builder.Services.TryAddScoped<IAccountRepository, AccountRepository>(); //Inject Dependency for registration
/*
 * Here we could be in a problem like if there is another Implement class which implements interface we need to write again the above line.
 * That also had a problem like the second one will override the first one
 * So we use something else
 * builder.Services.TryAddSingleton/TryAddScoped/TryAddTransient
 -------------------------TryAdd______--------
TryAddSingleton: This method attempts to add a service with the Singleton lifetime to the service collection. 
If a service of the same type has already been added, it won't be replaced. If the service isn't already registered, it will be added as a Singleton.

TryAddScoped: Similar to TryAddSingleton, this method attempts to add a service with the Scoped lifetime. 
If a service of the same type has already been added, it won't be replaced. If the service isn't already registered, it will be added as a Scoped service.

TryAddTransient: Like the previous methods, TryAddTransient attempts to add a service with the Transient 
lifetime. If a service of the same type has already been added, it won't be replaced. If the service isn't already registered, it will be added as a Transient service.

These methods are useful when you want to register services but want to avoid accidentally overwriting an existing registration of the same type. 
If the service is already registered, these methods will have no effect.

Here's an example of how these methods can be used:

csharp
Copy code
// Assuming you have an IServiceCollection instance named 'services'

// Try to add a Singleton service, but only if it's not already registered
services.TryAddSingleton<IMyService, MyService>();

// Try to add a Scoped service, but only if it's not already registered
services.TryAddScoped<IMyScopedService, MyScopedService>();

// Try to add a Transient service, but only if it's not already registered
services.TryAddTransient<IMyTransientService, MyTransientService>();
By using the "TryAdd" methods, you ensure that you don't accidentally overwrite existing registrations when configuring your service collection. 
This can be particularly helpful when you're working with third-party libraries that might also register services.


 */
//---------------------------------For DBCONTEXT-------------------------------------------
//builder.Services.AddDbContext<ProjectHeroContext>(options =>
// options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectHeroContext") ?? throw new InvalidOperationException("Connection string 'ProjectHeroContext' not found.")));
builder.Services.AddDbContext<ProjectHeroContext>(opt =>
 opt.UseSqlServer());
//--------------------------------------IDENTITY USER-----------------
//here we are adding our Identity and which will create tables in the database using Identity
builder.Services.AddIdentity<HeroUser, IdentityRole>() //The parameters here are 1st UserClass (HeroUser)
                                                       //2nd is IdentityRole(here we are using default one as we did not create our own IdentityClass
    .AddEntityFrameworkStores<ProjectHeroContext>()    //It shows it is working by using our DbContextClass(here ProjectHeroContext)
    .AddDefaultTokenProviders(); //This will provide necessary tokens as well and It is default one as we did not make our own.

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.RequireHttpsMetadata = false;
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,  // This line is duplicated, you can remove one of them
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ValidateLifetime = true, // Uncomment if you want to validate token lifetime
        ValidateIssuerSigningKey = true // Uncomment if you want to validate the issuer signing key
    };
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

//CORS 
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseCors("corspolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
