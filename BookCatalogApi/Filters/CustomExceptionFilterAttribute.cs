using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookCatalogApi.Filters
{
    public class CustomExceptionFilterAttribute: ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var result = new ObjectResult("An error occurred. This is Custom Exception Filter attribute handler")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.Result = result;
        }
    }
}
