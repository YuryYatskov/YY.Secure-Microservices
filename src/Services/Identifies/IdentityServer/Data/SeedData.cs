namespace IdentityServer.Data;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitialize>();
        dbInitializer.Initialize();
    }
}
