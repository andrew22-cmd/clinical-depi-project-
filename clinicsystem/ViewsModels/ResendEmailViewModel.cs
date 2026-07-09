using System.ComponentModel.DataAnnotations;

namespace clinicsystem.ViewModels
{
    public class ResendEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}