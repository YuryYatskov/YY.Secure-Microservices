using IdentityServer.Configurations;
using IdentityServer.Data;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer.Extensions;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        Config.Builder = builder;
        
        builder.Services.AddCors();

        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var filePath = Path.Combine(builder.Environment.ContentRootPath, "IS_certificate.pfx");
        var certificate = X509CertificateLoader.LoadPkcs12FromFile(filePath, "Qwe123@");
        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiResources(Config.ApiResources)           
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            //.AddDeveloperSigningCredential();
            .AddSigningCredential(certificate);

        builder.Services.AddScoped<IDatabaseInitialize, DatabaseInitialize>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors(builder =>
        {
            builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
        });

        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}

