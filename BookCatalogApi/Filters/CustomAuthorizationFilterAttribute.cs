using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookCatalogApi.Filters;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _value;
    private readonly string _key;
    public CustomAuthorizationFilterAttribute(string value, string key = "permission")
    {
        _value = value;
        _key = key;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if the user is authenticated
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check if the permission claim exists in the JWT claims
        var permissionClaim = context.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type.Equals(_key, StringComparison.OrdinalIgnoreCase)
                              && c.Value.Equals(_value, StringComparison.OrdinalIgnoreCase));

        if (permissionClaim == null)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
