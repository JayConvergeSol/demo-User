using System.ComponentModel.DataAnnotations;

namespace demoUser.Infrastructure.DTO
{
    public class ResetPasswordDTO
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OPT Is Required")]
        public string OTP { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
