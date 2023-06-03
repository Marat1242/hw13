using System.ComponentModel.DataAnnotations;

namespace Supermarket.ModelViews
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int CustomerId { get; set; }

        [Display(Name = "Current Password")]
        [Required(ErrorMessage = "Please enter current password")]
        public string PasswordNow { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Please enter a new password")]
        [MinLength(5, ErrorMessage = "You need to set a password of at least 5 characters")]
        public string Password { get; set; }

        [MinLength(5, ErrorMessage = "You need to set a password of at least 5 characters")]
        [Display(Name = "Retype Password")]
        [Compare("Password", ErrorMessage = "Invalid password re-entered")]
        public string ConfirmPassword { get; set; }
    }
}
