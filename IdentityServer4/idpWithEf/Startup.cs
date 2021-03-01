using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rsk.WsFederation;
using Rsk.WsFederation.Configuration;
using Rsk.WsFederation.EntityFramework.DbContexts;
using Rsk.WsFederation.EntityFramework.Mappers;
using Rsk.WsFederation.EntityFramework.Stores;
using Rsk.WsFederation.Models;

namespace idpWithEf
{
    public class Startup
    {
        private static readonly Client RelyingParty = new Client
        {
            ClientId = "rp1",
            AllowedScopes = {"openid", "profile"},
            RedirectUris = {"https://localhost:5001/signin-wsfed"},
            ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
        };

        private static readonly RelyingParty RelyingPartyOverrides = new RelyingParty
        {
            TokenType = WsFederationConstants.TokenTypes.Saml11TokenProfile11
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllersWithViews();

            // SAML SP database (DbContext)
            services.AddDbContext<WsFederationConfigurationDbContext>(db =>
                db.UseInMemoryDatabase("RelyingParties"));
            services.AddScoped<IWsFederationConfigurationDbContext, WsFederationConfigurationDbContext>();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddTestUsers(TestUsers.Users)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(new List<Client> {RelyingParty})
                .AddSigningCredential(new X509Certificate2("idsrv3test.pfx", "idsrv3test"))
                .AddWsFederationPlugin(options =>
                {
                    options.Licensee = "";
                    options.LicenseKey = "";
                })
                .AddRelyingPartyStore<RelyingPartyStore>();
                //.AddInMemoryRelyingParties(new List<RelyingParty>());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            SeedRelyingPartyDatabase(app);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer()
                .UseIdentityServerWsFederationPlugin();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        private void SeedRelyingPartyDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
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
}