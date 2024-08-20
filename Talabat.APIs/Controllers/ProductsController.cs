using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entites;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{

	public class ProductsController : ApiBaseController
	{
		private readonly IGenericRepository<Product> _productRepo;
		private readonly IGenericRepository<ProductType> _typeRepo;
		private readonly IGenericRepository<ProductBrand> _brandRepo;
		private readonly IMapper _mapper;

		public ProductsController(IGenericRepository<Product> productRepo, 
			IGenericRepository<ProductType> typeRepo ,
			IGenericRepository<ProductBrand> brandRepo
			, IMapper mapper)
		{
			_productRepo = productRepo;
			_typeRepo = typeRepo;
			_brandRepo = brandRepo;
			_mapper = mapper;
		}

		// Get All Products
		[Authorize]
		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams Params)
		{
			var spec = new ProductWithBrandAndTypeSpecifications(Params);	

			var products = await _productRepo.GetAllWithSpecAsync(spec);

			var mappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

			var countSpec = new ProductWithFiltrationForCountAsync(Params);

			var count = await _productRepo.GetCountWithSpecAsync(countSpec);

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

			var product = await _productRepo.GetByIdWithSpecAsync(spec);

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
			var types = await _typeRepo.GetAllAsync();

			return Ok(types);
		}


		// Get All Brands
		[HttpGet("Brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var brands = await _brandRepo.GetAllAsync();

			return Ok(brands);
		}
	}
}
