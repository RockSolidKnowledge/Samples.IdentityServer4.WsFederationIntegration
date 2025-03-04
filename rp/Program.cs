using rp;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
HostingExtensions.ConfigureServices(builder.Services);
WebApplication app = builder.Build();
HostingExtensions.Configure(app);
await app.RunAsync();