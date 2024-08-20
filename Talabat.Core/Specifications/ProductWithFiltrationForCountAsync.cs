using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites;

namespace Talabat.Core.Specifications
{
	public class ProductWithFiltrationForCountAsync : BaseSpecifications<Product>
	{
        public ProductWithFiltrationForCountAsync(ProductSpecParams Params) :
			base(p =>
			(!Params.brandId.HasValue || p.ProductBrandId == Params.brandId)
			&&
			(!Params.typeId.HasValue || p.ProductTypeId == Params.typeId)
			&&
			(string.IsNullOrEmpty(Params.Search) || p.Name.ToLower().Contains(Params.Search))
			)
		{
            
        }
    }
}
