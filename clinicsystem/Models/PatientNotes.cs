namespace clinicsystem.Models
{
    public class PatientNotes
    {
        public int NoteId { get; set; }

        public int ReservationId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public string Notes { get; set; }
        public string Diagnosis { get; set; }

        public DateTime VisitDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Reservation Reservation { get; set; }
    }
}
