using Application.Models;
using Domain.Entities;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Application.Abstraction;

public interface ITokenService
{
    public Task<Token> CreateTokensAsync(User user);
    public Task<Token> CreateTokensFromRefresh(ClaimsPrincipal principal, RefreshToken savedRefreshToken);
    public ClaimsPrincipal GetClaimsFromExpiredToken(string token);

    public Task<bool> AddRefreshToken(RefreshToken tokens);
    public bool Update(RefreshToken tokens);
    public IQueryable<RefreshToken> Get(Expression<Func<RefreshToken, bool>> predicate);
    public bool Delete(RefreshToken token);
}
