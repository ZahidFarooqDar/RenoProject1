using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
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
    // Set the default authentication scheme to JwtBearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(option =>
{
    // Indicate that the token should be saved in the authentication properties
    option.SaveToken = true;

    // Disable HTTPS metadata validation (only for development, not recommended in production)
/*    RequireHttpsMetadata is set to false, indicating that HTTPS metadata validation is disabled.
      This is typically done for development purposes and should be enabled in a production environment.*/
    option.RequireHttpsMetadata = false;

    // Configure token validation parameters
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        // Validate the issuer (typically the server that created the token)
        ValidateIssuer = true,

        // Validate the audience (typically the intended recipient of the token)
        ValidateAudience = true, 

        // Define the valid issuer and audience from configuration
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],

        // Set the issuer signing key used to validate the token's signature
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),

        // Validate the token's lifetime (expiration date)
        ValidateLifetime = true, // Uncomment if you want to validate token lifetime

        // Validate the issuer signing key
        ValidateIssuerSigningKey = true // Uncomment if you want to validate the issuer signing key
    };
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        /*
         * We were having problem that we were unable to send BearerToken in swagger, for enabling that we use this functionality
         */
        // options.SwaggerDoc("v1", new OpenApiInfo { Title = "Project Hero", Version = "DEMO" });
    options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "Demo",
            Title = "Project HERO",
            Description = "An ASP.NET Core Web API 6.0 for managing CRUD and Validation On HERO API",
            //TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Developer's LinkedIn Contact",
                Url = new Uri("https://www.linkedin.com/in/zahid-farooq-dar/")
            },
            License = new OpenApiLicense
            {
                Name = "Projet's Github Link",
                Url = new Uri("https://github.com/ZahidFarooqDar/RenoProject1/tree/main/ProjectHero")
            }
        });

        // Define a security scheme for bearer tokens
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        // Assign the security requirements to the operations
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    });
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
