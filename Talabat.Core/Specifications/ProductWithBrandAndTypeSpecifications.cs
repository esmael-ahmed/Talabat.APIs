using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites;

namespace Talabat.Core.Specifications
{
	public class ProductWithBrandAndTypeSpecifications : BaseSpecifications<Product>
	{
		// Get All Product
        public ProductWithBrandAndTypeSpecifications(ProductSpecParams Params) : 
			base(p => 
			(!Params.brandId.HasValue || p.ProductBrandId == Params.brandId)
			&&
			(!Params.typeId.HasValue || p.ProductTypeId == Params.typeId)
			&&
			(string.IsNullOrEmpty(Params.Search) || p.Name.ToLower().Contains(Params.Search))
			)
			
        {
            Includes.Add(p => p.ProductBrand);
			Includes.Add(p => p.ProductType);

			if (!string.IsNullOrEmpty(Params.sort))
			{
				switch (Params.sort)
				{
					case "priceAsec":
						AddOrderBy(p => p.Price);
						break;
					case "priceDesc":
						AddOrderByDescinding(p => p.Price);
						break;
					default:
						AddOrderBy(p => p.Name);
						break;

				}
			}

			ApplyPagination(Params.PageSize * (Params.PageIndex - 1), Params.PageSize);

			
		}

		// Get Product By Id
        public ProductWithBrandAndTypeSpecifications(int id) : base(p => p.Id == id)
        {
			Includes.Add(p => p.ProductBrand);
			Includes.Add(p => p.ProductType);
		}
    }
}
