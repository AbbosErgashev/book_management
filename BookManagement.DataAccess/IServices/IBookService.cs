using BookManagement.Models;
using BookManagement.Models.DTO;

namespace BookManagement.DataAccess.IServices;

public interface IBookService
{
    Task<Book> AddBookAsync(BookDto book);
    Task<List<Book>> AddBooksAsync(IEnumerable<BookDto> books);
    Task<Book> UpdateBookAsync(Guid id, BookDto book);
    Task<bool> SoftDeleteBookAsync(Guid id);
    Task<int> SoftDeleteBooksAsync(IEnumerable<Guid> ids);
    Task<IEnumerable<string>> GetPopularBooksAsync(int pageNumber, int pageSize);
    Task<Book> GetBookDetailsAsync(Guid id);
    Task<Book> GetBookByTitleAsync(string title);
}
