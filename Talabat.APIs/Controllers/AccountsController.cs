using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entites.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	
	public class AccountsController : ApiBaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountsController(UserManager<AppUser> userManager,
			SignInManager<AppUser> signInManager,
			ITokenService tokenService,
			IMapper mapper)
        {
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
			_mapper = mapper;
		}

        // Register
        [HttpPost("Register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			// check Email Dublication
			if(CheckEmailExist(model.Email).Result.Value == true) // use Result.Value => To make in Synchronous
			{
				return BadRequest(new ApiResponse(400, "The Email Is Already Exists"));
			}

			var user = new AppUser()
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				PhoneNumber = model.PhoneNumber,
				UserName = model.Email.Split('@')[0]
			};

			var result =  await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded)
			{
				return BadRequest(new ApiResponse(400));
			}

			var returnedUser = new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)
			};

			return Ok(returnedUser);
		}



		// Login

		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user is null)
			{
				return Unauthorized(new ApiResponse(401));
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

			if (!result.Succeeded)
			{
				return Unauthorized(new ApiResponse(401));
			}

			return Ok(new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)
			});

		}


		// Get Current User
		[Authorize]
		[HttpGet("GetCurrentUser")]
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);

			var user = await _userManager.FindByEmailAsync(email);

			var returnedUser = new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)
			};

			return Ok(returnedUser);
		}


		// Get Current User Address
		[Authorize]
		[HttpGet("Address")]
		public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
		{
			var user = await _userManager.FindUserWithAddressAsync(User);

			var mappedAddress = _mapper.Map<Address, AddressDto>(user.Address);

			return Ok(mappedAddress);

		}


		// update Current User Address
		[Authorize]
		[HttpPut("Address")]
		public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
		{
			var user = await _userManager.FindUserWithAddressAsync(User);

			var mappedAddress = _mapper.Map<AddressDto, Address>(addressDto);

			mappedAddress.Id = user.Address.Id;

			user.Address = mappedAddress;

			var result =  await _userManager.UpdateAsync(user);

			if (!result.Succeeded) return BadRequest(new ApiResponse(400));

			var updatedAddress = _mapper.Map<Address, AddressDto>(mappedAddress);

			return Ok(updatedAddress);
		}


		// Check Email Exist
		[HttpGet("EmailExists")]
		public async Task<ActionResult<bool>> CheckEmailExist(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null) return false;
			else return true;
		}
	}
}
