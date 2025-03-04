using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using Rsk.WsFederation.Configuration;
using Rsk.WsFederation.Models;

namespace idp;

internal static class HostingExtensions
{
    private static readonly Client RelyingParty = new()
    {
        ClientId = "rp1",
        AllowedScopes = { "openid", "profile" },
        RedirectUris = { "https://localhost:5001/signin-wsfed" },
        RequireConsent = false,
        ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
    };

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMvc();
        builder.Services.AddControllersWithViews();

        builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddTestUsers(TestUsers.Users)
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(new List<ApiResource>())
            .AddInMemoryClients(new List<Client> { RelyingParty })
            .AddSigningCredential(new X509Certificate2("idsrv3test.pfx", "idsrv3test"))
            .AddWsFederationPlugin(options =>
            {
                options.Licensee = "";
                options.LicenseKey = "";
            })
            .AddInMemoryRelyingParties(new List<RelyingParty>());

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseIdentityServer()
            .UseIdentityServerWsFederationPlugin();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

        return app;
    }
}