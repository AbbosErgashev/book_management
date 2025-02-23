using System.Text.Json.Serialization;

namespace BookManagement.Models;

public class Book
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("publication_year")]
    public int PublicationYear { get; set; }

    [JsonPropertyName("author_name")]
    public string AuthorName { get; set; } = string.Empty;

    [JsonPropertyName("view_counts")]
    public int ViewCounts { get; set; } = 0;
}
