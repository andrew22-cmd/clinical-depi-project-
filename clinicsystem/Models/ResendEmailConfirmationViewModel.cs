using System.ComponentModel.DataAnnotations;

namespace clinicsystem.ViewModels
{
    public class ResendEmailConfirmationViewModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة.")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;
    }
}
