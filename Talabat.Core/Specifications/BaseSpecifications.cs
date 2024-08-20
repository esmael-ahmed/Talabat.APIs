using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites;

namespace Talabat.Core.Specifications
{
	public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
	{
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
		public Expression<Func<T, object>> OrderByAsec { get; set; }
		public Expression<Func<T, object>> OrderByDesc { get ; set ; }
		public int Skip { get; set; }
		public int Take { get; set; }
		public bool IsPaginationEnabled { get; set; }


		// Get All
		public BaseSpecifications()
        {
            
        }

        // Get By Id
        public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }

        public void AddOrderBy(Expression<Func<T, object>> orderByAsec)
        {
            OrderByAsec = orderByAsec;
        }

		public void AddOrderByDescinding(Expression<Func<T, object>> orderByDesc)
        {
            OrderByDesc = orderByDesc;
        }

        public void ApplyPagination(int skip, int take)
        {
            IsPaginationEnabled = true;

            Skip = skip;

            Take = take;
        }


	}
}
