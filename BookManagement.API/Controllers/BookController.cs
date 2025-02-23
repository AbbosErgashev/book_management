using BookManagement.DataAccess.IServices;
using BookManagement.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BookController> _logger;

    public BookController(IBookService bookService, ILogger<BookController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    /// <summary>
    /// Get a list of popular books with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number for pagination.</param>
    /// <param name="pageSize">Number of books per page.</param>
    /// <returns>List of popular books.</returns>
    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularBooks(int pageNumber = 1, int pageSize = 10)
    {
        _logger.LogInformation("Getting popular books with pagination: page {PageNumber}, size {PageSize}", pageNumber, pageSize);
        var books = await _bookService.GetPopularBooksAsync(pageNumber, pageSize);
        return Ok(books);
    }

    /// <summary>
    /// Get details of a specific book.
    /// </summary>
    /// <param name="id">ID of the book.</param>
    /// <returns>Book details.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookDetails(Guid id)
    {
        _logger.LogInformation("Getting details for book with ID {BookId}", id);
        var book = await _bookService.GetBookDetailsAsync(id);
        if (book == null)
        {
            _logger.LogWarning("Book with ID {BookId} not found", id);
            return NotFound(new { Message = $"Book with ID {id} not found." });
        }
        return Ok(book);
    }

    /// <summary>
    /// Add a new book (single).
    /// </summary>
    /// <param name="bookDto">Book data to add.</param>
    /// <returns>Action result indicating status.</returns>
    [HttpPost("single")]
    public async Task<IActionResult> AddBook([FromBody] BookDto bookDto)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(bookDto.Title) || bookDto.PublicationYear < 0)
        {
            return BadRequest(new { Message = "Invalid book data." });
        }

        var addedBook = await _bookService.AddBookAsync(bookDto);
        if (addedBook == null)
        {
            return Conflict(new { Message = $"A book with the title '{bookDto.Title}' already exists." });
        }

        return CreatedAtAction(nameof(GetBookDetails), new { id = addedBook.Id }, addedBook);
    }

    /// <summary>
    /// Add multiple books (bulk).
    /// </summary>
    /// <param name="bookDtos">List of books to add.</param>
    /// <returns>Action result with added books count.</returns>
    [HttpPost("bulk")]
    public async Task<IActionResult> AddBooks([FromBody] List<BookDto> bookDtos)
    {
        if (bookDtos == null || bookDtos.Count == 0)
        {
            return BadRequest(new { Message = "No books provided." });
        }

        var addedBooks = await _bookService.AddBooksAsync(bookDtos);

        if (addedBooks.Count == 0)
        {
            return Conflict(new { Message = "All provided books already exist." });
        }

        return Ok(new { AddedCount = addedBooks.Count, Books = addedBooks });
    }

    /// <summary>
    /// Update an existing book (single).
    /// </summary>
    /// <param name="id">ID of the book to update.</param>
    /// <param name="bookDto">Updated book data.</param>
    /// <returns>Updated book details.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookDto bookDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Message = "Invalid book data." });
        }

        var updatedBook = await _bookService.UpdateBookAsync(id, bookDto);
        if (updatedBook == null)
        {
            return NotFound(new { Message = $"Book with ID {id} not found." });
        }

        return Ok(updatedBook);
    }

    /// <summary>
    /// Soft delete a book (single).
    /// </summary>
    /// <param name="id">ID of the book to delete.</param>
    /// <returns>No content if deleted.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteBook(Guid id)
    {
        var result = await _bookService.SoftDeleteBookAsync(id);
        if (!result)
        {
            return NotFound(new { Message = $"Book with ID {id} not found." });
        }

        return NoContent();
    }

    /// <summary>
    /// Soft delete books (bulk).
    /// </summary>
    /// <param name="ids">List of book IDs to delete.</param>
    /// <returns>Number of books deleted.</returns>
    [HttpDelete("bulk")]
    public async Task<IActionResult> SoftDeleteBooks([FromBody] IEnumerable<Guid> ids)
    {
        if (ids == null || !ids.Any())
        {
            return BadRequest(new { Message = "No book IDs provided." });
        }

        int deletedCount = await _bookService.SoftDeleteBooksAsync(ids);
        return Ok(new { DeletedCount = deletedCount });
    }
}
