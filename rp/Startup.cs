using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace rp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllersWithViews();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "cookie";
                    options.DefaultChallengeScheme = "wsfed";
                })
                .AddCookie("cookie")
                .AddWsFederation("wsfed", options =>
                {
                    options.MetadataAddress = "https://localhost:5000/wsfed";
                    options.Wtrealm = "rp1";
                    options.RequireHttpsMetadata = false;
                    options.SignInScheme = "cookie";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}