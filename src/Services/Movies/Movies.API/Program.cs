using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Movies.API.Data;
using Movies.API.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoviesContext>(options => { options.UseInMemoryDatabase("Movies"); });

builder.Services.AddScoped<IDatabaseInitialize, DatabaseInitialize>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Description = "Movies API v1",
        Title = "Movies API",
        Version = "1.0"
    });

    options.AddSecurityDefinition(SecuritySchemeType.OAuth2.GetDisplayName(),
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                ClientCredentials = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri($"{builder.Configuration["ApiSettings:IdentityAPIAddress"]!}/connect/token"),
                    Scopes = new Dictionary<string, string>() {
                        {
                            builder.Configuration["ApiSettings:Scope"]!,
                            builder.Configuration["ApiSettings:ScopeName"]!
                        }
                    }
                }
            }
        });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = SecuritySchemeType.OAuth2.GetDisplayName()
                },
                Scheme = SecuritySchemeType.OAuth2.GetDisplayName(),
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = builder.Configuration["ApiSettings:IdentityAPIAddress"]!;
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["ApiSettings:Key"]!)),
            ValidateIssuer = false,
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

        c.OAuthClientId(builder.Configuration["ApiSettings:ClientId"]!);
        c.OAuthClientSecret(builder.Configuration["ApiSettings:Key"]!);
        c.OAuthScopes(builder.Configuration["IdentityServer:ApiScopes"]);
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
