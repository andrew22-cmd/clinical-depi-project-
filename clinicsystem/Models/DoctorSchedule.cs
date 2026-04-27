namespace clinicsystem.Models
{
    public class DoctorSchedule
    {
        public int ScheduleId { get; set; }

        public int DoctorId { get; set; }

        public string WeekDay { get; set; }

        public bool IsActive { get; set; }

        // Navigation
        public Doctor Doctor { get; set; }

        public ICollection<DoctorScheduleSlot> Slots { get; set; }
    }
}
