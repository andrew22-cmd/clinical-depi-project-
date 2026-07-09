namespace clinicsystem.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }

        public int UserId { get; set; }
        public int SpecialityId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal ConsultationFee { get { return 400m + (SpecialityId * 50m); } }

        // Navigation
        public User User { get; set; }
        public Speciality Speciality { get; set; }

        public ICollection<DoctorSchedule> Schedules { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
