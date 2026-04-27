namespace clinicsystem.Models
{
    public class DoctorScheduleSlot
    {
        public int SlotId { get; set; }

        public int ScheduleId { get; set; }

        public TimeSpan SlotTime { get; set; }

        public bool IsBooked { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public DoctorSchedule Schedule { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
