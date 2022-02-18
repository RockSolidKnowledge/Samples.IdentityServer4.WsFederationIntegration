using Duende.IdentityServer.Configuration;
using Microsoft.EntityFrameworkCore;
using Rsk.WsFederation.Configuration;
using Rsk.WsFederation.EntityFramework.DbContexts;
using Rsk.WsFederation.EntityFramework.Mappers;
using Rsk.WsFederation.EntityFramework.Stores;
using Serilog;

namespace idpWithEf;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        // WsFed database (DbContext)
        builder.Services.AddDbContext<WsFederationConfigurationDbContext>(db =>
            db.UseInMemoryDatabase("RelyingParties"));
        builder.Services.AddScoped<IWsFederationConfigurationDbContext, WsFederationConfigurationDbContext>();

        builder.Services.AddIdentityServer(options =>
        {
            options.KeyManagement.Enabled = true;
            options.KeyManagement.SigningAlgorithms = new[] {
                    new SigningAlgorithmOptions("RS256") {UseX509Certificate = true}
                };

            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
            options.EmitStaticAudienceClaim = true;
        })
            .AddTestUsers(TestUsers.Users)
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddWsFederationPlugin(options =>
            {
                options.Licensee = "";
                options.LicenseKey = "";
            })
            .AddRelyingPartyStore<RelyingPartyStore>();

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
        app.UseDeveloperExceptionPage();

        app.SeedRelyingPartyDatabase();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer()
            .UseIdentityServerWsFederationPlugin();

        app.UseAuthorization();
        
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }

    private static void SeedRelyingPartyDatabase(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<WsFederationConfigurationDbContext>();
            if (!context.RelyingParties.Any())
            {
                context.RelyingParties.Add(Config.RelyingPartyOverrides.ToEntity());
                context.SaveChanges();
            }
        }
    }
}