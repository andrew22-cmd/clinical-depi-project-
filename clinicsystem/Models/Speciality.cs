namespace clinicsystem.Models
{
    public class Speciality
    {
        public int SpecialityId { get; set; }

        public string Name { get; set; }

        /// <summary>Fee charged per reservation for this speciality (EGP)</summary>
        public decimal ConsultationFee { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }
}
