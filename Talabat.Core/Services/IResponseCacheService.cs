using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Services
{
	public interface IResponseCacheService
	{
		// Cache Response In Memory
		Task CacheResponseAsync(string key, object response, TimeSpan timeToLive);

		// Get Cached Response From Memory
		Task<string?> GetCachedResponseAsync(string key);
	}
}
