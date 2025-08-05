using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.API.Data;
using Movies.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoviesContext>(options => { options.UseInMemoryDatabase("Movies"); });

builder.Services.AddScoped<IDatabaseInitialize, DatabaseInitialize>();

builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = builder.Configuration["ApiSettings:IdentityAPIAddress"]!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies API V1");
        c.RoutePrefix = string.Empty;
    });
}

SeedData.EnsureSeedData(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/movies", async (MoviesContext db) =>
    await db.Movies.ToListAsync())
    .RequireAuthorization();

app.MapGet("/movies/{id}", async (int id, MoviesContext db) =>
    await db.Movies.FindAsync(id));

app.MapPost("/movies", async (Movie movie, MoviesContext db) =>
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();
    return Results.Created($"/Movies/{movie.Id}", movie);
});

app.MapPut("/movies/{id}", async (int id, Movie updatedItem, MoviesContext db) =>
{

    var movie = await db.Movies.FindAsync(id);
    if (movie is null) return Results.NotFound();

    movie.Title = updatedItem.Title;
    movie.Genre = updatedItem.Genre;
    movie.Rating = updatedItem.Rating;
    movie.ReleaseDate = updatedItem.ReleaseDate;
    movie.ImageUrl = updatedItem.ImageUrl;
    movie.Owner = updatedItem.Owner;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/movies/{id}", async (int id, MoviesContext db) =>
{
    if (await db.Movies.FindAsync(id) is Movie movie)
    {
        db.Movies.Remove(movie);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();
