using System.Linq.Expressions;

namespace Application.Repositories;

public interface IRepository<T>
{
    public IEnumerable<T> Get(Expression<Func<T, bool>> predicate);

    public Task<T> GetByIdAsync(int Id);

    public Task<T> AddAsync(T entity);
    public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> sync);
    public Task<T> UpdateAsync(T sync);

    public Task<bool> DeleteAsync(int id);

}
