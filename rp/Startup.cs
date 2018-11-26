using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace rp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "cookie";
                    options.DefaultChallengeScheme = "wsfed";
                })
                .AddCookie("cookie")
                .AddWsFederation("wsfed", options =>
                {
                    options.MetadataAddress = "http://localhost:5000/wsfed";
                    options.Wtrealm = "rp1";
                    options.RequireHttpsMetadata = false;
                    options.SignInScheme = "cookie";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}