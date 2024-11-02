using Microsoft.AspNetCore.Mvc.Filters;

namespace BookCatalogApi.Filters
{
    public class DateTimeExecutionFilterAttribute : Attribute, IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            context.HttpContext.Response.Headers.Add("DateTime", DateTime.Now.ToString());
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
        }
    }
}
