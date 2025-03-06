namespace rp;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMvc();
        builder.Services.AddControllersWithViews();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "cookie";
                options.DefaultChallengeScheme = "wsfed";
            })
            .AddCookie("cookie")
            .AddWsFederation("wsfed", options =>
            {
                options.MetadataAddress = "https://localhost:5000/signin-wsfed";
                options.Wtrealm = "rp1";
                options.RequireHttpsMetadata = false;
                options.SignInScheme = "cookie";
            });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // Map routes
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}