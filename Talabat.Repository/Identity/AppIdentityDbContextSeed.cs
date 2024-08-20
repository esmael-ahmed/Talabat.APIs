using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Identity;

namespace Talabat.Repository.Identity
{
	public static class AppIdentityDbContextSeed
	{
		public static async Task SeedUserAsync(UserManager<AppUser> userManager)
		{
			if (!userManager.Users.Any())
			{
				var user = new AppUser()
				{
					DisplayName = "Esmael Ahmed",
					Email = "esmailhelal2929@gmail.com",
					UserName = "esmailhelal2929",
					PhoneNumber = "01234567891"
				};
				await userManager.CreateAsync(user, "Pa$$w0rd");
			}
		}
	}
}
