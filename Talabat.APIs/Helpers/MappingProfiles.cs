using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entites;
using Talabat.Core.Entites.Identity;

namespace Talabat.APIs.Helpers
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles() 
		{
			CreateMap<Product, ProductToReturnDto>()
				.ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
				.ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
				.ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>());

			CreateMap<Address, AddressDto>().ReverseMap();

			CreateMap<CustomerBasketDto, CustomerBasket>();
			CreateMap<BasketItemDto, BasketItem>();
		}
	}
}
