using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
	
	public class BuggyController : ApiBaseController
	{
		private readonly StoreContext _dbContext;

		public BuggyController(StoreContext dbContext) 
		{
			_dbContext = dbContext;
		}


		[HttpGet("NotFound")]

		public ActionResult GetNotFoundRequest()
		{
			var product = _dbContext.Products.Find(100);

			if (product == null)
			{
				return NotFound(new ApiResponse(404));
			}

			return Ok(product);
		}


		[HttpGet("ServerError")]
		public ActionResult GetServerError()
		{
			var product = _dbContext.Products.Find(100);
			var productToReturn = product.ToString();
			return Ok(productToReturn);
		}

		[HttpGet("BadRequest")]
		public ActionResult GetBadRequest()
		{
			return BadRequest(new ApiResponse(400));
		}

		[HttpGet("BadRequest/{id}")]
		public ActionResult GetBadRequest(int id)
		{
			return Ok();
		}

	}
}
