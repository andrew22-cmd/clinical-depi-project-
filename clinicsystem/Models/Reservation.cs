namespace clinicsystem.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int SlotId { get; set; }

        public DateTime ReservationDate { get; set; }

        public string Status { get; set; }

        public string Source { get; set; } // online / offline

        public int? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public DoctorScheduleSlot Slot { get; set; }

        public ICollection<PatientNotes> Notes { get; set; }
        public ICollection<EmailNotification> Notifications { get; set; }
    }
}
