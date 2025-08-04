using Microsoft.EntityFrameworkCore;
using Movies.API.Models;

namespace Movies.API.Data;

public class MoviesContext(DbContextOptions<MoviesContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies { get; set; }
}
