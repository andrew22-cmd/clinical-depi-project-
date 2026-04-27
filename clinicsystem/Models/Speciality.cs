namespace clinicsystem.Models
{
    public class Speciality
    {
        public int SpecialityId { get; set; }

        public string Name { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }
}
