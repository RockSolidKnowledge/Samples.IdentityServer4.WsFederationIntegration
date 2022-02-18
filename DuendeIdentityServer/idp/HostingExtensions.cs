using Duende.IdentityServer.Configuration;
using Rsk.WsFederation.Configuration;
using Rsk.WsFederation.Models;
using Serilog;

namespace idp;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

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
            .AddInMemoryRelyingParties(new List<RelyingParty>());

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer()
            .UseIdentityServerWsFederationPlugin();

        app.UseAuthorization();
        
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}