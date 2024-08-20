using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.MiddleWares;
using Talabat.Core.Entites;
using Talabat.Core.Entites.Identity;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.APIs
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<StoreContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefultConnection"));
			});

			builder.Services.AddDbContext<AppIdentityDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
			});

			//builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();

			builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
			{
				var connection = builder.Configuration.GetConnectionString("RedisConnection");
				return ConnectionMultiplexer.Connect(connection);
			});

			builder.Services.AddApplicationServices();

			builder.Services.AddIdentityServices(builder.Configuration);

			var app = builder.Build();




			#region Update-DataBase
			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;

			var loggerFactory = services.GetRequiredService<ILoggerFactory>();	
			
			try
			{
				var dbContext = services.GetRequiredService<StoreContext>(); // update database
				await dbContext.Database.MigrateAsync();

				var IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>(); // update database for identity
				await IdentityDbContext.Database.MigrateAsync();

				var userManger = services.GetRequiredService<UserManager<AppUser>>();
				await AppIdentityDbContextSeed.SeedUserAsync(userManger);

				await StoreContextSeed.SeedAsync(dbContext);
			}
			catch(Exception ex)
			{
				var logger = loggerFactory.CreateLogger<Program>();
				logger.LogError(ex, "An Error Occured During Appling The Migration");
			}
			#endregion




			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMiddleware<ExceptionMiddleWare>();

				app.UseSwaggerMiddlewares();
			}

			app.UseStaticFiles();

			app.UseStatusCodePagesWithReExecute("/errors/{0}");

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
