﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Identity;
using Talabat.Core.Services;

namespace Talabat.Service
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration configuration;

		public TokenService(IConfiguration configuration)
        {
			this.configuration = configuration;
		}
        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
		{
			var authClaims = new List<Claim>()
			{
				new Claim(ClaimTypes.GivenName, user.DisplayName),
				new Claim(ClaimTypes.Email, user.Email)
			};

			var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
				authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

			var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

			var token = new JwtSecurityToken(issuer: configuration["Jwt:ValidIssure"],
											 audience: configuration["Jwt:ValidAudience"],
											 expires: DateTime.Now.AddDays(double.Parse(configuration["Jwt:DurationInDays"])),
											 claims : authClaims,
											 signingCredentials : new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature));

			return new JwtSecurityTokenHandler().WriteToken(token);
        }
	}
}
