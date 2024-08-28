using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories
{
	public interface IGenericRepository<T> where T : BaseEntity
	{
		#region With out Specifications 
		Task<IReadOnlyList<T>> GetAllAsync();

		Task<T> GetByIdAsync(int id);
		#endregion

		#region With Specifications
		// Get All
		Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);

		// Get By Id
		Task<T> GetEntityWithSpecAsync(ISpecifications<T> spec);

		Task<int> GetCountWithSpecAsync(ISpecifications<T> spec);

		// Add
		#endregion
		Task AddAsync(T Item);

		void Update(T Item);
		void Delete(T Item);
	}
}
