using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entites;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{

	public class ProductsController : ApiBaseController
	{
		
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ProductsController(IUnitOfWork unitOfWork
			, IMapper mapper)
		{
			
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		// Get All Products
		[Authorize]
		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams Params)
		{
			var spec = new ProductWithBrandAndTypeSpecifications(Params);	

			var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

			var mappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

			var countSpec = new ProductWithFiltrationForCountAsync(Params);

			var count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(countSpec);

			return Ok(new Pagination<ProductToReturnDto>(Params.PageSize, Params.PageIndex, mappedProducts, count));
		}

		// Get Product By Id
		[Authorize]
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
		{
			var spec = new ProductWithBrandAndTypeSpecifications(id);	

			var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

			if (product == null)
			{
				return NotFound(new ApiResponse(404));
			}

			var mappedProduct = _mapper.Map<Product, ProductToReturnDto>(product);	
			return Ok(mappedProduct);
		}


		// Get All Types
		[HttpGet("Types")]
		public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
		{
			var types = await _unitOfWork.Repository<ProductType>().GetAllAsync();

			return Ok(types);
		}


		// Get All Brands
		[HttpGet("Brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();

			return Ok(brands);
		}
	}
}
