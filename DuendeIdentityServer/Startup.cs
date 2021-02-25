// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Duende.IdentityServer.Configuration;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.WsFederation.Configuration;
using Rsk.WsFederation.Models;
using System.Collections.Generic;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace DuendeIdP
{
    public class Startup
    {
        private static readonly Client RelyingParty = new Client
        {
            ClientId = "rp1",
            AllowedScopes = { "openid", "profile" },
            RedirectUris = { "https://localhost:5001/signin-wsfed" },
            RequireConsent = false,
            ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddIdentityServer(options =>
            {
                options.KeyManagement.Enabled = true;
                options.KeyManagement.SigningAlgorithms = new[] {
                    new SigningAlgorithmOptions("RS256") {UseX509Certificate = true}
                };

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
                .AddTestUsers(TestUsers.Users)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiResources(new List<ApiResource>())
                .AddInMemoryClients(new List<Client> { RelyingParty })
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