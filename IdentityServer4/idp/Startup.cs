using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.WsFederation.Configuration;
using Rsk.WsFederation.Models;

namespace idp
{
    public class Startup
    {
        private static readonly Client RelyingParty = new Client
        {
            ClientId = "rp1",
            AllowedScopes = {"openid", "profile"},
            RedirectUris = {"https://localhost:5001/signin-wsfed"},
            RequireConsent = false,
            ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllersWithViews();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddTestUsers(TestUsers.Users)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(new List<ApiResource>())
                .AddInMemoryClients(new List<Client> {RelyingParty})
                .AddSigningCredential(new X509Certificate2("idsrv3test.pfx", "idsrv3test"))
                .AddWsFederationPlugin(options =>
                {
                    options.Licensee = "";
                    options.LicenseKey = "";
                })
                .AddInMemoryRelyingParties(new List<RelyingParty>());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer()
               .UseIdentityServerWsFederationPlugin();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}