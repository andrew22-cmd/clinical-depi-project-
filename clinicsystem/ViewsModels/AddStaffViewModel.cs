using clinicsystem.Models;
using System.ComponentModel.DataAnnotations;

namespace clinicsystem.ViewModels
{
    public class AddStaffViewModel
    {
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الأخير مطلوب")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "يجب تحديد نوع الوظيفة")]
        public string Role { get; set; } = string.Empty; // "Doctor" أو "Secretary"

        // التخصص هيكون اختياري لأنه مطلوب للدكتور فقط
        public int? SpecialityId { get; set; }

        public IEnumerable<Speciality> Specialities { get; set; } = new List<Speciality>();
    }
}