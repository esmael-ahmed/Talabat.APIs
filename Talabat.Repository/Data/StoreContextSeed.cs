using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entites;
using Talabat.Core.Entites.Order_Aggregate;

namespace Talabat.Repository.Data
{
	public static class StoreContextSeed
	{
		public static async Task SeedAsync(StoreContext dbContext)
		{
			if (!dbContext.ProductBrands.Any())
			{
				var BrandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
				var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);

				if (Brands?.Count > 0)
				{
					foreach (var Brand in Brands)
					{
						await dbContext.ProductBrands.AddAsync(Brand);
					}
					await dbContext.SaveChangesAsync();
				}
				
				
			}

			if (!dbContext.ProductTypes.Any())
			{
				var TypesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/types.json");
				var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);

				if (Types?.Count > 0)
				{
					foreach (var type in Types)
					{
						await dbContext.ProductTypes.AddAsync(type);
					}
					await dbContext.SaveChangesAsync();
				}


			}

			if (!dbContext.Products.Any())
			{
				var ProductsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
				var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);

				if (Products?.Count > 0)
				{
					foreach (var product in Products)
					{
						await dbContext.Products.AddAsync(product);
					}
					await dbContext.SaveChangesAsync();
				}


			}


			if (!dbContext.DeliveryMethods.Any())
			{
				var DeliveryMethodsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");
				var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);

				if (DeliveryMethods?.Count > 0)
				{
					foreach (var DeliveryMethod in DeliveryMethods)
					{
						await dbContext.DeliveryMethods.AddAsync(DeliveryMethod);
					}
					await dbContext.SaveChangesAsync();
				}


			}
		}
	}
}
