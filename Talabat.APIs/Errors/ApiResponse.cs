
namespace Talabat.APIs.Errors
{
	public class ApiResponse
	{
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefultMessageForStatusCode(statusCode);
        }

		private string? GetDefultMessageForStatusCode(int statusCode)
		{
			return statusCode switch
			{
				400 => "BadRequest",
				401 => "You Are Not Authorized",
				404 => "Resource Not Found",
				500 => "Internal Server Error",
				_ => null
			};
		}
	}
}
