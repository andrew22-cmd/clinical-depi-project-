using clinicsystem.Models;

namespace clinicsystem.ViewModels
{
    public class EditStaffViewModel
    {
        public int Id { get; set; } // يمكن أن يكون UserId أو DoctorId حسب التصميم
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Doctor أو Secretary

        public int? SpecialityId { get; set; } // في حالة كان طبيب
        public IEnumerable<Speciality> Specialities { get; set; } = new List<Speciality>();
    }
}