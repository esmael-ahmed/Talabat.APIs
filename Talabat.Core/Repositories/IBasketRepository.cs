using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites;

namespace Talabat.Core.Repositories
{
	public interface IBasketRepository
	{
		// GetBasket
		Task<CustomerBasket?> GetBasketAsync(string BasketId);

		// Create Or Update
		Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket Basket);

		// Delete
		Task<bool> DeleteBasketAsync(string BasketId);


	}
}
