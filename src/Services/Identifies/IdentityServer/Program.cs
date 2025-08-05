var builder = WebApplication.CreateBuilder(args);

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

SeedData.EnsureSeedData(app);

app.Run();