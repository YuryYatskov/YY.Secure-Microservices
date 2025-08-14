using Refit;
using WebUI.Models;

namespace WebUI.Services;

public interface IMovieService
{
    [Get("/movies")] // "/movie-service/movies")]
    Task<GetMoviesResponse> GetMovies();
}
