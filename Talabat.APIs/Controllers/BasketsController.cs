using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entites;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
	public class BasketsController : ApiBaseController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;

		public BasketsController(IBasketRepository basketRepository,
								 IMapper mapper)
        {
			_basketRepository = basketRepository;
			_mapper = mapper;
		}

        // Get Or ReCreat eBasket

        [HttpGet("{basketId}")]
		public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string basketId)
		{
			var basket = await _basketRepository.GetBasketAsync(basketId);

			return basket is null ? new CustomerBasket(basketId) : basket;
		}


		// Update Or Create Basket

		[HttpPost]
		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
		{
			var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
			var updatedOrCreatedBasket =  await _basketRepository.UpdateBasketAsync(mappedBasket);

			return updatedOrCreatedBasket is null ? BadRequest(new ApiResponse(400)) : Ok(updatedOrCreatedBasket); 
		}


		// Delete Basket

		[HttpDelete]
		public async Task<ActionResult<bool>> DeleteBasket(string basketId)
		{
			return await _basketRepository.DeleteBasketAsync(basketId);
		}
	}
}
