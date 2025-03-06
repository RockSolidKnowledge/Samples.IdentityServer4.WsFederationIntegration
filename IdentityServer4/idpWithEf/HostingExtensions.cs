using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using Microsoft.EntityFrameworkCore;
using Rsk.WsFederation;
using Rsk.WsFederation.Configuration;
using Rsk.WsFederation.EntityFramework.DbContexts;
using Rsk.WsFederation.EntityFramework.Mappers;
using Rsk.WsFederation.EntityFramework.Stores;
using Rsk.WsFederation.Models;

namespace idpWithEf;

internal static class HostingExtensions
{
    private static readonly Client RelyingParty = new()
    {
        ClientId = "rp1",
        AllowedScopes = { "openid", "profile" },
        RedirectUris = { "https://localhost:5001/signin-wsfed" },
        ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
    };

    private static readonly RelyingParty RelyingPartyOverrides = new()
    {
        Realm = "rp1",
        TokenType = WsFederationConstants.TokenTypes.Saml2TokenProfile11
    };

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMvc();
        builder.Services.AddControllersWithViews();

        // WsFed database (DbContext)
        builder.Services.AddDbContext<WsFederationConfigurationDbContext>(db =>
            db.UseInMemoryDatabase("RelyingParties"));
        builder.Services.AddScoped<IWsFederationConfigurationDbContext, WsFederationConfigurationDbContext>();

        builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddTestUsers(TestUsers.Users)
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(Config.GetApis())
            .AddInMemoryClients(new List<Client> { RelyingParty })
            .AddSigningCredential(new X509Certificate2("idsrv3test.pfx", "idsrv3test"))
            .AddWsFederationPlugin(options =>
            {
                options.Licensee = "";
                options.LicenseKey = "";
            })
            .AddRelyingPartyStore<RelyingPartyStore>();
        //.AddInMemoryRelyingParties(new List<RelyingParty>());

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseDeveloperExceptionPage();

        SeedRelyingPartyDatabase(app);

        app.UseStaticFiles();

        app.UseRouting();

        app.UseIdentityServer()
            .UseIdentityServerWsFederationPlugin();

        app.UseAuthorization();

        // Map routes
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");


        return app;
    }

    private static void SeedRelyingPartyDatabase(IApplicationBuilder app)
    {
        using (IServiceScope? serviceScope =
               app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<WsFederationConfigurationDbContext>();
            if (!context.RelyingParties.Any())
            {
                context.RelyingParties.Add(RelyingPartyOverrides.ToEntity());
                context.SaveChanges();
            }
        }
    }
}