﻿using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entites;
using Talabat.Core.Entites.Identity;
using Talabat.Core.Entites.Order_Aggregate;

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

			CreateMap<Core.Entites.Identity.Address, AddressDto>().ReverseMap();

			CreateMap<AddressDto, Core.Entites.Order_Aggregate.Address>();

			CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
			CreateMap<BasketItemDto, BasketItem>().ReverseMap();

			CreateMap<Order, OrderToReturnDto>()
				.ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
				.ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));

			CreateMap<OrderItem, OrderItemDto>()
				.ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
				.ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
				.ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.Product.PictureUrl))
				.ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemPictureUrlResolver>());
		}
	}
}
