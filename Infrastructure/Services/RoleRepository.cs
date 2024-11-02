using Application.Abstraction;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class RoleRepository : IRoleRepository
{
    private readonly IBookCatalogDbContext _bookCatalogDbContext;

    public RoleRepository(IBookCatalogDbContext bookCatalogDbContext)
    {
        _bookCatalogDbContext = bookCatalogDbContext;
    }

    public async Task<Role> AddAsync(Role role)
    {
        _bookCatalogDbContext.Roles.Add(role);
        int res = await _bookCatalogDbContext.SaveChangesAsync();
        if (res > 0)
        {
            return role;
        }
        return null;
    }

    public async Task<IEnumerable<Role>> AddRangeAsync(IEnumerable<Role> roles)
    {
        _bookCatalogDbContext.Roles.AttachRange(roles);

        int res = await _bookCatalogDbContext.SaveChangesAsync();

        if (res > 0) { return roles; }

        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Role? role = await _bookCatalogDbContext.Roles.FindAsync(id);
        if (role != null)
        {
            _bookCatalogDbContext.Roles.Remove(role);
        }
        int res = await _bookCatalogDbContext.SaveChangesAsync();
        if (res > 0)
            return true;
        return false;
    }

    public IEnumerable<Role> Get(Expression<Func<Role, bool>> predicate)
    {
        return _bookCatalogDbContext.Roles.Where(predicate).Include(x => x.Permissions);
    }

    public async Task<Role?> GetByIdAsync(int Id)
    {
        return _bookCatalogDbContext.Roles.Where(x => x.RoleId == Id).Include(x => x.Permissions).SingleOrDefault();
    }

    public async Task<Role> UpdateAsync(Role updatedRole)
    {
        var existingRole = await GetByIdAsync(updatedRole.RoleId);
        if (existingRole != null)
        {
            existingRole.Name = updatedRole.Name;

            existingRole.Permissions.Clear();
            foreach (var permission in updatedRole.Permissions)
            {
                // Assuming the PermissionId is unique and exists in the database
                var existingPermission = _bookCatalogDbContext.Permissions.Find(permission.PermissionId);
                if (existingPermission != null)
                {
                    existingRole.Permissions.Add(existingPermission);
                }
            }

            // Step 4: Save the changes to the database
            int res = await _bookCatalogDbContext.SaveChangesAsync();
            if (res > 0)
            {
                return updatedRole;
            }
        }
        return null;

    }
}
