using BookManagement.DataAccess;
using BookManagement.DataAccess.IServices;
using BookManagement.Models;
using BookManagement.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.API.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _context;
    private readonly ILogger<BookService> _logger;

    public BookService(AppDbContext context, ILogger<BookService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Book> AddBookAsync(BookDto bookDto)
    {
        var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Title == bookDto.Title);
        if (existingBook is not null)
        {
            _logger.LogWarning("Book with title '{Title}' already exists.", bookDto.Title);
            return null;
        }

        var newBook = new Book
        {
            Id = Guid.NewGuid(),
            Title = bookDto.Title,
            PublicationYear = bookDto.PublicationYear,
            AuthorName = bookDto.AuthorName,
            ViewCounts = bookDto.ViewCounts
        };

        _context.Books.Add(newBook);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Book with title '{Title}' successfully added.", bookDto.Title);
        return newBook;
    }

    public async Task<List<Book>> AddBooksAsync(IEnumerable<BookDto> bookDtos)
    {
        var existingTitles = await _context.Books
            .Select(b => b.Title.ToLower())
            .ToListAsync();

        var newBooks = bookDtos
            .Where(b => !string.IsNullOrWhiteSpace(b.Title) && b.PublicationYear > 0)
            .Where(b => !existingTitles.Contains(b.Title.ToLower()))
            .Select(b => new Book
            {
                Id = Guid.NewGuid(),
                Title = b.Title,
                PublicationYear = b.PublicationYear,
                AuthorName = b.AuthorName,
                ViewCounts = b.ViewCounts
            })
            .ToList();

        if (newBooks.Count != 0)
        {
            _context.Books.AddRange(newBooks);
            await _context.SaveChangesAsync();
            _logger.LogInformation("{Count} books successfully added.", newBooks.Count);
        }
        else
        {
            _logger.LogWarning("No new books to add.");
        }

        return newBooks;
    }

    public async Task<Book> UpdateBookAsync(Guid id, BookDto bookDto)
    {
        var existingBook = await _context.Books.FindAsync(id);
        if (existingBook is null)
        {
            _logger.LogWarning("Book with ID '{Id}' not found for update.", id);
            return null;
        }

        existingBook.Title = bookDto.Title;
        existingBook.PublicationYear = bookDto.PublicationYear;
        existingBook.AuthorName = bookDto.AuthorName;
        existingBook.ViewCounts = bookDto.ViewCounts;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Book with ID '{Id}' successfully updated.", id);
        return existingBook;
    }

    public async Task<bool> SoftDeleteBookAsync(Guid id)
    {
        var existingBook = await _context.Books.FindAsync(id);
        if (existingBook is null)
        {
            _logger.LogWarning("Book with ID '{Id}' not found for soft delete.", id);
            return false;
        }

        _context.Books.Remove(existingBook);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Book with ID '{Id}' successfully soft deleted.", id);
        return true;
    }

    public async Task<int> SoftDeleteBooksAsync(IEnumerable<Guid> ids)
    {
        var books = await _context.Books.Where(b => ids.Contains(b.Id)).ToListAsync();
        _context.Books.RemoveRange(books);
        await _context.SaveChangesAsync();
        _logger.LogInformation("{Count} books successfully soft deleted.", books.Count);
        return books.Count;
    }

    public async Task<IEnumerable<string>> GetPopularBooksAsync(int pageNumber, int pageSize)
    {
        var books = await _context.Books
            .OrderByDescending(b => b.ViewCounts)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => b.Title)
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} popular books for page {PageNumber}.", books.Count, pageNumber);
        return books;
    }

    public async Task<Book> GetBookDetailsAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            _logger.LogWarning("Book with ID '{Id}' not found.", id);
        }
        return book;
    }

    public async Task<Book> GetBookByTitleAsync(string title)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == title);
        if (book == null)
        {
            _logger.LogWarning("No book found with title '{Title}'.", title);
        }
        return book;
    }
}
