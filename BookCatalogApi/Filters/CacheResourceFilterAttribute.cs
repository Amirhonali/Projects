using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace BookCatalogApi.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheResourceFilterAttribute : Attribute, IResourceFilter
    {
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey;

        public CacheResourceFilterAttribute(string cacheKey)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _cacheKey = cacheKey;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (_cache.TryGetValue(_cacheKey, out var cachedResult))
            {
                context.Result = cachedResult as IActionResult;
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // Cache the result if it's an IActionResult
            if (context.Result is IActionResult result)
            {
                var option = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(10));

                // Cache the result using the specified cache key
                _cache.Set(_cacheKey, result, option);
            }
        }

        //public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        //{
        //    if (_cache.TryGetValue(_cacheKey, out var cachedResult))
        //    {
        //        context.Result = cachedResult as IActionResult;
        //    }
        //    next();
        //    // Cache the result if it's an IActionResult
        //    if (context.Result is IActionResult result)
        //    {
        //        var option = new MemoryCacheEntryOptions()
        //            .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
        //            .SetSlidingExpiration(TimeSpan.FromSeconds(10));

        //        // Cache the result using the specified cache key
        //        _cache.Set(_cacheKey, result, option);
        //    }

        //    return Task.CompletedTask;
        //}
    }
}
