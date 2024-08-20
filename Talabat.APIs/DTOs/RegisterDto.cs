using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
	public class RegisterDto
	{
		[Required]
		[EmailAddress]
        public string Email { get; set; }

		[Required]
        public string DisplayName { get; set; }

		[Required]
		[Phone]
        public string PhoneNumber { get; set; }

		[Required]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
			ErrorMessage ="Password must contain 1 uppercase, 1 lowercase, 1 digit, 1 special character")]
        public string Password { get; set; }
    }
}
