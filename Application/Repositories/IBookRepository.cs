using Domain.Entities;

namespace Application.Repositories;

public interface IBookRepository:IRepository<Book>
{
    public IEnumerable<Book> SearchBooks(string text);
}
