using Application.Abstraction;
using Application.Extensions;
using Application.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IBookCatalogDbContext _dbContext;
    private readonly int _refreshTokenLifetime;
    private readonly int _accessTokenLifetime;

    public TokenService(IConfiguration configuration, IBookCatalogDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _refreshTokenLifetime = int.Parse(configuration["JWT:RefreshTokenLifetime"]);
        _accessTokenLifetime = int.Parse(configuration["JWT:AccessTokenLifetime"]);
    }

    public async Task<Token> CreateTokensAsync(User user)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Email, user.Email)
        };
        Role? role1;
        List<string> permissions = new();
        foreach (Role role in user.Roles)
        {
            role1 = _dbContext.Roles.Where(x => x.RoleId.Equals(role.RoleId)).Include(x => x.Permissions).SingleOrDefault();
            foreach (Permission permission in role1.Permissions)
            {
                if (!permissions.Contains(permission?.PermissionName))
                {
                    permissions.Add(permission?.PermissionName);
                    claims.Add(new Claim("Permission", permission?.PermissionName));
                }
            }
        }
        Token tokens = CreateToken(claims);

        RefreshToken? SavedRefreshToken = Get(x => x.Email == user.Email).FirstOrDefault();
        if (SavedRefreshToken == null)
        {
            var refreshToken = new RefreshToken()
            {
                ExpiredDate = DateTime.UtcNow.AddMinutes(_refreshTokenLifetime),
                RefreshTokenValue = tokens.RefreshToken,
                Email = user.Email
            };
            await AddRefreshToken(refreshToken);

        }
        else
        {
            SavedRefreshToken.RefreshTokenValue = tokens.RefreshToken;
            SavedRefreshToken.ExpiredDate = DateTime.UtcNow.AddMinutes(_refreshTokenLifetime);
            Update(SavedRefreshToken);
        }

        return tokens;
    }

    public Task<Token> CreateTokensFromRefresh(ClaimsPrincipal principal, RefreshToken savedRefreshToken)
    {
        Token tokens = CreateToken(principal.Claims);

        savedRefreshToken.RefreshTokenValue = tokens.RefreshToken;
        savedRefreshToken.ExpiredDate = DateTime.UtcNow.AddMinutes(_refreshTokenLifetime);

        Update(savedRefreshToken);
        return Task.FromResult(tokens);
    }


    public ClaimsPrincipal GetClaimsFromExpiredToken(string token)
    {
        byte[] Key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

        var tokenParams = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidAudience = _configuration["JWT:AudienceKey"],
            ValidateIssuer = true,
            ValidateLifetime = false,
            ValidIssuer = _configuration["JWT:IssuerKey"],
            IssuerSigningKey = new SymmetricSecurityKey(Key),
            ClockSkew = TimeSpan.Zero
        };

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenParams, out SecurityToken securityToken);
        JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null)// ||
                                     //jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }
        return principal;
    }

    public async Task<bool> AddRefreshToken(RefreshToken tokens)
    {
        await _dbContext.RefreshToken.AddAsync(tokens);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public bool Update(RefreshToken tokens)
    {
        _dbContext.RefreshToken.Update(tokens);
        _dbContext.SaveChangesAsync();
        return true;
    }

    public IQueryable<RefreshToken> Get(Expression<Func<RefreshToken, bool>> predicate)
    {
        return _dbContext.RefreshToken.Where(predicate);
    }

    public bool Delete(RefreshToken token)
    {
        _dbContext.RefreshToken.Remove(token);
        _dbContext.SaveChangesAsync();
        return true;
    }


    private Token CreateToken(IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:IssuerKey"],
            audience: _configuration["JWT:AudienceKey"],
            expires: DateTime.UtcNow.AddMinutes(_accessTokenLifetime),
            claims: claims,
            signingCredentials: new SigningCredentials(
                                    new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                                        SecurityAlgorithms.HmacSha256Signature)
            );
        Token tokens = new()
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = GenerateRefreshToken()
        };
        return tokens;
    }

    private string GenerateRefreshToken()
    {
        return (DateTime.UtcNow.ToString() + _configuration["JWT:Key"]).GetHash();
    }
}
