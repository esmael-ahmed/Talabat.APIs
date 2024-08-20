using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entites;
using Talabat.Core.Repositories;

namespace Talabat.Repository
{
	public class BasketRepository : IBasketRepository
	{
		private readonly IDatabase _database;

		public BasketRepository(IConnectionMultiplexer Redis)
        {
			_database = Redis.GetDatabase();
		}
        public async Task<bool> DeleteBasketAsync(string BasketId)
		{
			return await _database.KeyDeleteAsync(BasketId);
		}

		public async Task<CustomerBasket?> GetBasketAsync(string BasketId)
		{
			var basket = await _database.StringGetAsync(BasketId);
			return basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
		}

		public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket Basket)
		{
			var jsonbasket = JsonSerializer.Serialize(Basket);
			var createdOrDeleted = await _database.StringSetAsync(Basket.Id, jsonbasket, TimeSpan.FromDays(1));

			if (!createdOrDeleted) { return null; }
			else
				return await GetBasketAsync(Basket.Id);
		}
	}
}
