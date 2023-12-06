using System.ComponentModel.DataAnnotations;

namespace BizNewsAppDB1.AuthDTO
{
    public class RegisterDTO
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public IFormFile Photo { get; set; }
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
