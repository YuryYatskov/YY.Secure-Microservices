namespace WebUI.Models;

public class MovieModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public string Rating { get; set; } = string.Empty;

    public DateTime ReleaseDate { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string Owner { get; set; } = string.Empty;
}

// Wrapper classes.
public record GetMoviesResponse(IEnumerable<MovieModel> Movies);