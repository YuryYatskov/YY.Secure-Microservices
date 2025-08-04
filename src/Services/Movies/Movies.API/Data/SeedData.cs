namespace Movies.API.Data;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDatabaseInitialize>();
        db.Initialize();
    }
}
