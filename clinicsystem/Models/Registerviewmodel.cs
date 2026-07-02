using System.ComponentModel.DataAnnotations;

namespace clinicsystem.ViewModels
{
 
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "الاسم الأول مطلوب.")]
        [Display(Name = "الاسم الأول")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم العائلة مطلوب.")]
        [Display(Name = "اسم العائلة")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;


        [Required(ErrorMessage = "رقم الهاتف مطلوب.")]
        [Display(Name = "رقم الهاتف")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [StringLength(40, MinimumLength = 8,
            ErrorMessage = "كلمة المرور يجب أن تكون بين 8 و 40 حرفاً.")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين.")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}