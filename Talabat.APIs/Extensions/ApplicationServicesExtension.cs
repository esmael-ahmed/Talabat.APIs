using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Repository;

namespace Talabat.APIs.Extensions
{
	public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
		{
			Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

			Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

			Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			Services.AddAutoMapper(typeof(MappingProfiles));

			Services.Configure<ApiBehaviorOptions>(option =>
				option.InvalidModelStateResponseFactory = (ActionContext) =>
				{
					var errors = ActionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
														 .SelectMany(p => p.Value.Errors)
														 .Select(e => e.ErrorMessage).ToArray();

					var validationErrorResponse = new ApiValidationErrorResponse()
					{
						Errors = errors
					};

					return new BadRequestObjectResult(validationErrorResponse);
				}
			);

			return Services;
		}
	}
}
