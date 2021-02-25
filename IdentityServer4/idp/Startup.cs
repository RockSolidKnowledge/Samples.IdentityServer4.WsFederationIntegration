using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.WsFederation.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
                    options.Licensee = "DEMO";
                    options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMDItMThUMDA6MDA6MDQuMzM2OTU4NSswMDowMCIsImlhdCI6IjIwMjEtMDEtMTlUMDA6MDA6MDQiLCJvcmciOiJERU1PIiwiYXVkIjozfQ==.k40OZZfH6bp6WFIpXLdKvYaiT+TggMRx0vGtvNGIf4z90U9YOFOXzINW7b8ceFrce20flFLlM7RRfEb43dzTuDI59GHC6WALLDqRd2ngRcvTsU5u74zsiCZcbDlZ6jh2jzBQD7JCxdMiZ7FfOS8Nu0F9Ib9KnwrnsaKPaDY37EJSfUl1lmrM0CfFFJoutuWVObyG62fEe9+BQjrPC1IUfkGFxKNyZlsXDcvKHXNHP6mWPhKFOiKhXDZ98Awwi4ZlOhbo/FJ1mkNr/B9lSF/vdLa27rBipkTaZN0yN7nL5qkjTattfedAzumtmmbCtrpsoH4ordTd/W4YqMYgMZvJOMyH5iY7Zlw53dl/TJRSmrqxl1QnO1LMyKhiWbu81tH7vUE7M9Kf8XlSOyJ0//iiFt+WuOlK3wx9ChqfI2aeItPu8wSS2ML5IrHxc4fAKiXu7glCdpQy8NVF6RLrdjyLSL7eHp3xF4XfxbKH5tkLhbbcNHnzekuncnh65qFD20rQUB8jFdh7pTeqI32uvT16TsarheSgg9gQJaWSvBJRh2YYDJzrkRZh4uq4W9/SRzXR5eM1EvlKY4WXSCnLxSyIfQBb19QqzcGC/airkkQ1x7AhS1bYPoauNpxpeQUZVjabJuiINEeR8NPkqJPbrw/YrrXhKiem6pZg3WfEe1g78nU=";
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