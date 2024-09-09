using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.APIs.Helpers
{
	public class CachedAttribute : Attribute, IAsyncActionFilter
	{
		private readonly int _timeToLiveInSeconed;

		public CachedAttribute(int timeToLiveInSeconed)
        {
			_timeToLiveInSeconed = timeToLiveInSeconed;
		}
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
			// Ask clr for object from ResponseCacheService Explicitly

			var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

			var response = await responseCacheService.GetCachedResponseAsync(cacheKey);

			// Cached
			if (!string.IsNullOrEmpty(response))
			{
				var result = new ContentResult()
				{
					Content = response,
					ContentType = "application/json",
					StatusCode = 200
				};

				context.Result = result;
				return;
			}

			// Not Cached
			var excutedActionContext =  await next.Invoke();

			if (excutedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
			{
				await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconed));
			}

		}

		private string GenerateCacheKeyFromRequest(HttpRequest request)
		{
			var keyBuilder = new StringBuilder();

			keyBuilder.Append(request.Path);

            foreach (var (key, value) in request.Query.OrderBy(q => q.Key))
            {
				keyBuilder.Append($"|{key}-{value}");
            }
			return keyBuilder.ToString();
        }
	}
}
