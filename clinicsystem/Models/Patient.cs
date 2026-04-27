namespace clinicsystem.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        public int? UserId { get; set; } // nullable عشان offline

        public string FullName { get; set; }
        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
