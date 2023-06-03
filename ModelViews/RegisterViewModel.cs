using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Supermarket.ModelViews
{
    public class RegisterViewModel
    {
        [Key]
        public int CustomerId { get; set; }

        [Display(Name = "First and Last Name")]
        [Required(ErrorMessage = "Please Enter Full Name")]
        public string FullName { get; set; }

        [MaxLength(150)]
        [Required(ErrorMessage = "Please Enter Email")]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Email { get; set; }

        [MaxLength(11)]
        [Required(ErrorMessage = "Please enter phone number\"")]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }

		[MaxLength(150)]
		[Required(ErrorMessage = "Please Enter Address")]
		[DataType(DataType.Text)]
		[Remote(action: "ValidateEmail", controller: "Accounts")]
		public string Address { get; set; }


		[Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter password")]
        [MinLength(5, ErrorMessage = "You need to set a password of at least 5 characters")]
        public string Password { get; set; }

        [MinLength(5, ErrorMessage = "You need to set a password of at least 5 characters")]
        [Display(Name = "Re-enter Password")]
        [Compare("Password", ErrorMessage = "Re-enter incorrect password\"")]
        public string ConfirmPassword { get; set; }
    }
}
