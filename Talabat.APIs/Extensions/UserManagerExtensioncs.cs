using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entites.Identity;

namespace Talabat.APIs.Extensions
{
	public static class UserManagerExtensioncs
	{
		public static async Task<AppUser?> FindUserWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal User)
		{
			var email = User.FindFirstValue(ClaimTypes.Email);

			//var user = await userManager.Users.Where(u => u.Email == email).Include(u => u.Address).FirstOrDefaultAsync();

			var user = await userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email);

			return user;
		}
	}
}
