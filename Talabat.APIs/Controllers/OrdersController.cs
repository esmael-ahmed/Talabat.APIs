using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entites.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	
	public class OrdersController : ApiBaseController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public OrdersController(IOrderService orderService,
								IMapper mapper,
								IUnitOfWork unitOfWork)
        {
			_orderService = orderService;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

        // CreateOrder
        [HttpPost]
		[Authorize]
		[ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var mappedAddress = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);
			var order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, mappedAddress);
			if (order == null )
			{
				return BadRequest(new ApiResponse(400, "There Is a Problem With Your Order"));
			}
			return Ok(order);	
		
		}



		// GetOrdersForSpecificUser
		[HttpGet]
		[Authorize]
		[ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForSpecificUser()
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var orders = await _orderService.GetOrdersForSpecificUserAsync(buyerEmail);
			if (orders == null )
			{
				return NotFound(new ApiResponse(404, "There is no orders for this user"));
			}
			var mappedOrders = _mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderToReturnDto>>(orders);
			return Ok(mappedOrders);
		}



		// GetOrderByIdForSpecificUser
		[HttpGet("{orderId}")]
		[Authorize]
		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForSpecificUser(int  orderId)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var order = await _orderService.GetOrderByIdForSpecificUserAsync(buyerEmail, orderId);
			if (order == null)
			{
				return NotFound(new ApiResponse(404, "There Is No Order For This Id"));
			}
			var mappedOrder = _mapper.Map<Order, OrderToReturnDto>(order);
			return Ok(mappedOrder);
		}



		// Get All DeliveryMethods
		[HttpGet("DeliveryMethods")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
		{
			var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
			return Ok(deliveryMethods);
		}
	}
}
