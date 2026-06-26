namespace clinicsystem.ViewModels
{
    // ---- Doctors & schedules listing ----
    public class ScheduleDayVM
    {
        public int ScheduleId { get; set; }
        public string WeekDay { get; set; } = "";
        public bool IsActive { get; set; }
        public string From { get; set; } = "-";
        public string To { get; set; } = "-";
        public List<string> Slots { get; set; } = new();
    }

    public class DoctorListItemVM
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = "";
        public string Speciality { get; set; } = "";
        public List<ScheduleDayVM> Days { get; set; } = new();
    }

    // ---- Add schedule (create doctor + first schedule) ----
    public class AddScheduleVM
    {
        public string Name { get; set; } = "";
        public string Speciality { get; set; } = "";
        public string Day { get; set; } = "";
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public string? SlotsCsv { get; set; }
    }

    // ---- Edit schedule ----
    public class DoctorOptionVM
    {
        public int DoctorId { get; set; }
        public string Label { get; set; } = "";
    }

    public class EditScheduleVM
    {
        public List<DoctorOptionVM> Doctors { get; set; } = new();
        public int DoctorId { get; set; }
        public string Day { get; set; } = "";
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public string? SlotsCsv { get; set; }
    }

    // ---- Create staff (manager-add-staff) ----
    public class CreateStaffVM
    {
        public string Role { get; set; } = "طبيب";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? Speciality { get; set; }
    }

    // ---- Reservation ----
    public class ReservationIndexVM
    {
        public List<SpecialityOptionVM> Specialities { get; set; } = new();
    }

    public class SpecialityOptionVM
    {
        public int SpecialityId { get; set; }
        public string Name { get; set; } = "";
    }

    public class BookRequest
    {
        public int SlotId { get; set; }
        public string PatientName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? Email { get; set; }
    }
}
