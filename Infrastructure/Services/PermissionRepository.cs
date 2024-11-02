using Application.Abstraction;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.DataAccess;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class PermissionRepository : IPermissionRepository
{
    private readonly IBookCatalogDbContext _bookCatalogDb;

    public PermissionRepository(IBookCatalogDbContext bookCatalogDb)
    {
        _bookCatalogDb = bookCatalogDb;
    }

    public async Task<Permission> AddAsync(Permission permission)
    {
        _bookCatalogDb.Permissions.Add(permission);

        int res = await _bookCatalogDb.SaveChangesAsync();
        if (res > 0) return permission;

        return null;
    }

    public async Task<IEnumerable<Permission>> AddRangeAsync(IEnumerable<Permission> permissions)
    {
        _bookCatalogDb.Permissions.AttachRange(permissions);
        int res = await _bookCatalogDb.SaveChangesAsync();
        if (res > 0) return permissions;

        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Permission? permission = await _bookCatalogDb.Permissions.FindAsync(id);
        if (permission != null)
        {
            _bookCatalogDb.Permissions.Remove(permission);
            int res = await _bookCatalogDb.SaveChangesAsync();
            if (res > 0) return true;
        }
        return false;

    }

    public IEnumerable<Permission> Get(Expression<Func<Permission, bool>> predicate)
    {
        return _bookCatalogDb.Permissions.Where(predicate).AsEnumerable();
    }

    public Task<Permission?> GetByIdAsync(int Id)
    {
        return Task.FromResult(_bookCatalogDb.Permissions.FindAsync(Id).Result);
    }

    public async Task<Permission> UpdateAsync(Permission permission)
    {
        _bookCatalogDb.Permissions.Update(permission);
        int res = await _bookCatalogDb.SaveChangesAsync();
        if (res > 0) return permission;
        return null;
    }
}
