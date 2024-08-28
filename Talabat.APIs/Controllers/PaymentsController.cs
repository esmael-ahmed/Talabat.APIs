using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entites;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	
	public class PaymentsController : ApiBaseController
	{
		private readonly IPaymentService _paymentService;
		private readonly IMapper _mapper;

		public PaymentsController(IPaymentService paymentService,
								  IMapper mapper)
        {
			_paymentService = paymentService;
			_mapper = mapper;
		}


		// Create Or Update EndPoint
		[Authorize]
		[HttpPost]
		[ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var customerBasket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);	
			if (customerBasket == null)
			{
				return BadRequest(new ApiResponse(400, "There is a Problem With Your Basket"));
			}
			var mappedBasket = _mapper.Map<CustomerBasket,CustomerBasketDto>(customerBasket);
			return Ok(mappedBasket);
		}



		// Stripe WebHook EndPoint
		[HttpPost("webhook")]
		public async Task<IActionResult> StripeWebHook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			const string endpointSecret = "whsec_yC2UZ3nwUPHE3SXCpfWQbWPP64I96KT2";
			try
			{
				var stripeEvent = EventUtility.ParseEvent(json);
				var signatureHeader = Request.Headers["Stripe-Signature"];

				stripeEvent = EventUtility.ConstructEvent(json,
						signatureHeader, endpointSecret);

				if (stripeEvent.Type == Events.PaymentIntentSucceeded)
				{
					var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
					Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);
					// Then define and call a method to handle the successful payment intent.
					// handlePaymentIntentSucceeded(paymentIntent);
					await _paymentService.UpdatePaymentIntentToSucceddOrFailed(paymentIntent.Id, true);

				}
				else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
				{
					var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
					// Then define and call a method to handle the successful attachment of a PaymentMethod.
					// handlePaymentMethodAttached(paymentMethod);
					await _paymentService.UpdatePaymentIntentToSucceddOrFailed(paymentIntent.Id, false);
				}
				else
				{
					Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
				}
				return Ok();
			}
			catch (StripeException e)
			{
				Console.WriteLine("Error: {0}", e.Message);
				return BadRequest();
			}
			catch (Exception e)
			{
				return StatusCode(500);
			}
		}
	}
}
