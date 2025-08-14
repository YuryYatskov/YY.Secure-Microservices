using Refit;
using WebUI.Models;

namespace WebUI.Services;

public interface IMovieService
{
    [Get("/movie-service/movies")] // "/movie-service/movies" // "/movies"
    Task<GetMoviesResponse> GetMovies();
}
