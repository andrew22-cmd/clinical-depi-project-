using System.ComponentModel.DataAnnotations;

namespace clinicsystem.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}