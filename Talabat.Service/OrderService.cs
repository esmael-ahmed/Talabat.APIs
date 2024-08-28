using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entites;
using Talabat.Core.Entites.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPaymentService _paymentService;

		public OrderService(IBasketRepository basketRepo,
							IUnitOfWork unitOfWork,
							IPaymentService paymentService)
        {
			_basketRepo = basketRepo;
			_unitOfWork = unitOfWork;
			_paymentService = paymentService;
		}
        async Task<Order?> IOrderService.CreateOrderAsync(string buyerEmail, string basketId, int deliveryMwthodId, Address shippingAddress)
		{
			// 1- Get Basket From BasketRepo
			var basket = await _basketRepo.GetBasketAsync(basketId);

			// 2- Get Selected Items at Basket From ProductRepo
			var orderItems = new List<OrderItem>();

			if(basket?.Items.Count > 0)
			{
                foreach (var item in basket.Items)
                {
					var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
					var productItemOrderd = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
					var orderItem = new OrderItem(productItemOrderd, item.Quantity, product.Price);
					orderItems.Add(orderItem);
                }
            }

			// 3- Calculate SubTotal
			var subTotal = orderItems.Sum(item => item.Quantity * item.Price);

			// 4- Get DeliveryMethod From DeliveryMethodRepo
			var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMwthodId);

			// 5- Create Order
			var spec = new OrderWithPaymentIntentId(basket.PaymentIntentId);
			var exOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
			if (exOrder is not null)
			{
				_unitOfWork.Repository<Order>().Delete(exOrder);
				await _paymentService.CreateOrUpdatePaymentIntent(basketId);
			}

			var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal, basket.PaymentIntentId);

			// 6- Add Order Locally
			await _unitOfWork.Repository<Order>().AddAsync(order);

			// 7- Save Order To DataBase
			var result =  await _unitOfWork.CompleteAsync();
			if(result <= 0)
			{
				return null;
			}
				
			return order;
		}

		async Task<Order> IOrderService.GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
		{
			var orderSpec = new OrderSpecifications(buyerEmail, orderId);
			var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(orderSpec);
			return order;
		}

		async Task<IReadOnlyList<Order>> IOrderService.GetOrdersForSpecificUserAsync(string buyerEmail)
		{
			var orderSpec = new OrderSpecifications(buyerEmail);
			var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(orderSpec);
			return orders;
		}
	}
}
