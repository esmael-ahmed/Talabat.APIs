using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly StoreContext _dbContext;

		public GenericRepository(StoreContext dbContext)
        {
			_dbContext = dbContext;
		}


		#region WithOut Spec
		public async Task<IReadOnlyList<T>> GetAllAsync()
		{

			return await _dbContext.Set<T>().ToListAsync();
		}



		public async Task<T> GetByIdAsync(int id)
		{
			return await _dbContext.Set<T>().FindAsync(id);
		}
		#endregion



		#region With Spec
		public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
		{
			return await ApplySpecification(spec).ToListAsync();
		}


		public async Task<T> GetByIdWithSpecAsync(ISpecifications<T> spec)
		{
			return await ApplySpecification(spec).FirstOrDefaultAsync();
		}
		#endregion


		private IQueryable<T> ApplySpecification(ISpecifications<T> spec)
		{
			return SpecificationEvalutor<T>.GetQuery(_dbContext.Set<T>(), spec);
		}

		public async Task<int> GetCountWithSpecAsync(ISpecifications<T> spec)
		{
			return await ApplySpecification(spec).CountAsync();
		}

		public async Task AddAsync(T Item)
		{
			  await _dbContext.Set<T>().AddAsync(Item);	
		}
	}
}
