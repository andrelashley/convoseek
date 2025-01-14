using System.ComponentModel.DataAnnotations;

namespace ConvoSeekBackend.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string? Handle { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
