namespace BookManagement.Models.DTO;

public class BookDto
{
    public string Title { get; set; } = null!;
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int ViewCounts { get; set; } = 0;
}