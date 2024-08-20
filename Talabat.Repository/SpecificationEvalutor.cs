﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
	public static class SpecificationEvalutor<T> where T : BaseEntity
	{
		public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> spec)
		{
			var query = inputQuery;

			if (spec.Criteria is not null)
			{
				query = query.Where(spec.Criteria);
			}

			if (spec.OrderByAsec is not null)
			{
				query = query.OrderBy(spec.OrderByAsec);
			}

			if (spec.OrderByDesc is not null)
			{
				query = query.OrderByDescending(spec.OrderByDesc);
			}

			if (spec.IsPaginationEnabled == true)
			{
				query = query.Skip(spec.Skip).Take(spec.Take);
			}

			query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

			return query;
		}
	}
}
