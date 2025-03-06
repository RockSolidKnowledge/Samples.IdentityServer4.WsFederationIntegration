using rp;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.ConfigureServices();
app.ConfigurePipeline();
await app.RunAsync();