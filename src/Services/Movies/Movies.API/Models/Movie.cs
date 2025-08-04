namespace Movies.API.Models;

public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public string Rating { get; set; } = string.Empty;

    public DateTime ReleaseDate{ get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string Owner { get; set; } = string.Empty;
}
