using Application.Abstraction;
using Application.Repositories;
using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class BookRepository : IBookRepository
{
    private readonly IBookCatalogDbContext _bookCatalogDb;

    public BookRepository(IBookCatalogDbContext bookCatalogDb)
    {
        _bookCatalogDb = bookCatalogDb;
    }

    public async Task<Book> AddAsync(Book book)
    {
        _bookCatalogDb.Books.Add(book);

        int res = await _bookCatalogDb.SaveChangesAsync();
        if (res > 0) return book;

        return null;
    }

    public async Task<IEnumerable<Book>> AddRangeAsync(IEnumerable<Book> books)
    {
        _bookCatalogDb.Books.AttachRange(books);
        int res = await _bookCatalogDb.SaveChangesAsync();
        if (res > 0) return books;

        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Book? book = await _bookCatalogDb.Books.FindAsync(id);
        if (book != null)
        {
            _bookCatalogDb.Books.Remove(book);
            int res = await _bookCatalogDb.SaveChangesAsync();
            if (res > 0) return true;
        }
        return false;


    }

    public IEnumerable<Book> Get(Expression<Func<Book, bool>> predicate)
    {
        return _bookCatalogDb.Books.Where(predicate);
    }

    public async Task<Book> GetByIdAsync(int Id)
    {
        return await _bookCatalogDb.Books.FindAsync(Id);
    }

    public IEnumerable<Book> SearchBooks(string text)
    {
        return _bookCatalogDb.Books.ToList().Where(x => x.ISBN.Contains(text) ||
                                         x.Name.Contains(text) ||
                                         x.PublishDate.ToString().Contains(text) ||
                                         x.Category.ToString().Contains(text)||
                                         x.Description.Contains(text));
    }
    public async Task<Book> UpdateAsync(Book book)
    {
        _bookCatalogDb.Books.Update(book);
        int res = await _bookCatalogDb.SaveChangesAsync();
        if (res > 0) return book;
        return null;
    }
}
