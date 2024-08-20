using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Order_Aggregate;

namespace Talabat.Core.Services
{
	public interface IOrderService
	{
		Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMwthodId, Address shippingAddress);
		Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail);
		Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId);
	}
}
