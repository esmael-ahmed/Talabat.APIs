using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.MiddleWares
{
	public class ExceptionMiddleWare
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleWare> _logger;
		private readonly IHostEnvironment _env;

		public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger, IHostEnvironment env)
        {
			_next = next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next.Invoke(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				// Production => log ex in database

				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

				//if (_env.IsDevelopment())
				//{
				//	var response = new ApiExceptionResponce((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString());
				//}
				//else
				//{
				//	var response = new ApiExceptionResponce((int)HttpStatusCode.InternalServerError);
				//}

				var response = _env.IsDevelopment() ? new ApiExceptionResponce((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) : new ApiExceptionResponce((int)HttpStatusCode.InternalServerError);
				var option = new JsonSerializerOptions()
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				};

				var jsonResponse = JsonSerializer.Serialize(response, option);
				context.Response.WriteAsync(jsonResponse);

			}
		}
    }
}
