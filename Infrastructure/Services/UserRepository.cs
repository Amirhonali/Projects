using Application.Abstraction;
using Application.Extensions;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class UserRepository : IUserRepository
{
    private readonly IBookCatalogDbContext _bookCatalogDb;

    public UserRepository(IBookCatalogDbContext bookCatalogDb)
    {
        _bookCatalogDb = bookCatalogDb;
    }

    public async Task<User> AddAsync(User user)
    {
        user.Password = user.Password.GetHash();
        _bookCatalogDb.Users.Add(user);

        int res = await _bookCatalogDb.SaveChangesAsync();
        if (res > 0) return user;

        return null;
    }

    public Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> sync)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        User? user = await _bookCatalogDb.Users.FindAsync(id);
        if (user != null)
        {
            _bookCatalogDb.Users.Remove(user);
            int res = await _bookCatalogDb.SaveChangesAsync();
            if (res > 0) return true;
        }
        return false;
    }

    public IEnumerable<User> Get(Expression<Func<User, bool>> predicate)
    {
        return _bookCatalogDb.Users.Where(predicate).Include(x=>x.Roles).ToList();
    }

    public Task<User?> GetByIdAsync(int Id)
    {
        return Task.FromResult(_bookCatalogDb.Users.Where(x=>x.Id.Equals(Id))
            .Include(x=>x.Roles)
            .SingleOrDefault());
    }

    public async Task<User> UpdateAsync(User user)
    {
        var existingUser = await GetByIdAsync(user.Id);
        if (existingUser != null)
        {
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;

            existingUser.Roles.Clear();
            foreach (var permission in user.Roles)
            {
                var existingPermission = _bookCatalogDb.Roles.Find(permission.RoleId);
                if (existingPermission != null)
                {
                    existingUser.Roles.Add(existingPermission);
                }
            }

            int res = await _bookCatalogDb.SaveChangesAsync();
            if (res > 0)
            {
                return user;
            }
        }
        return null;
    }
}
