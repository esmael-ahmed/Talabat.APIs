using Microsoft.Extensions.Configuration;
using Stripe;
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
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentService(IConfiguration configuration,
							  IBasketRepository basketRepository,
							  IUnitOfWork unitOfWork)
        {
			_configuration = configuration;
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
		}
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
		{
			// Secret Key
			StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];

			// Get Basket
			var basket = await _basketRepository.GetBasketAsync(basketId);
			if (basket is null) return null;
			var shippingPrice = 0M; // Decimal

			if(basket.DeliveryMethodId.HasValue)
			{
				var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
				shippingPrice = deliveryMethod.Cost;

			}

			// total = subtotal + DeliveryMethod Cost
			if(basket.Items.Count > 0)
			{
				foreach (var item in basket.Items)
				{
					var product = await _unitOfWork.Repository<Core.Entites.Product>().GetByIdAsync(item.Id);
					if(item.Price != product.Price)
					{
						item.Price = product.Price;
					}
				}
			}

			var subtotal = basket.Items.Sum(i => i.Price * i.Quantity);

			// Create PaymentIntent

			var service = new PaymentIntentService();
			PaymentIntent paymentIntent;

			if(string.IsNullOrEmpty(basket.PaymentIntentId)) //Create
			{
				var options = new PaymentIntentCreateOptions()
				{
					Amount = (long)subtotal * 100 + (long)shippingPrice * 100,
					Currency = "usd",
					PaymentMethodTypes = new List<string>() { "card" }
				};

				paymentIntent = await service.CreateAsync(options);
				basket.PaymentIntentId = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;
			}
			else //Update
			{
				var options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)subtotal * 100 + (long)shippingPrice * 100,

				};

				paymentIntent = await service.UpdateAsync(basket.PaymentIntentId, options);
				basket.PaymentIntentId = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;
			}

			await _basketRepository.UpdateBasketAsync(basket);
			return basket;
		}

		public async Task<Core.Entites.Order_Aggregate.Order> UpdatePaymentIntentToSucceddOrFailed(string paymentIntentId, bool flag)
		{
			var spec = new OrderWithPaymentIntentId(paymentIntentId);
			var order  = await _unitOfWork.Repository<Core.Entites.Order_Aggregate.Order>().GetEntityWithSpecAsync(spec);
			if(flag)
			{
				order.Status = OrderStatus.PaymentReceived;
			}
			else
			{
				order.Status = OrderStatus.PaymentFailed;
			}
			 _unitOfWork.Repository<Core.Entites.Order_Aggregate.Order>().Update(order);
			await _unitOfWork.CompleteAsync();
			return order;
		}
	}
}
