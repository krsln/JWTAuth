using JWTAuth.Models;

namespace JWTAuth.Services;

public interface IBookService
{
    public Task<List<Book>> GetBooksAsync();
    public Task<Book?> GetBooksByIdAsync(BookRequest req);
}

public class BookService : IBookService
{
    static readonly List<Book> _books =
    [
        new Book
        {
            BookId = 1, ISBN = "9752115047", Title = "22/11/63", AuthorName = "Stephen King",
            Description = "22 Kasım 1963’te, bütün bunları değiştirme şansınız olsaydı?"
        },

        new Book
        {
            BookId = 2, ISBN = "1476762740", Title = "Uyuyan Güzeller", AuthorName = "Stephen King *  Owen King",
            Description = "Şimdi burada dünyanın kaderine karar verilecek."
        },

        new Book
        {
            BookId = 3, ISBN = "9752126049", Title = "Enstitü", AuthorName = "Stephen King", Description = "Enstitü..."
        }
    ];

    public Task<List<Book>> GetBooksAsync()
    {
        return Task.FromResult(_books);
    }

    public Task<Book?> GetBooksByIdAsync(BookRequest req)
    {
        var loadedBookInformation = _books.FirstOrDefault(p => p.BookId == req.BookId);

        return Task.FromResult<Book?>(loadedBookInformation);
    }
}