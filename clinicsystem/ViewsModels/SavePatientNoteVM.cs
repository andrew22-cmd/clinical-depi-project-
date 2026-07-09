namespace clinicsystem.ViewModels
{
    public class SavePatientNoteVM
    {
        public int ReservationId { get; set; }

        public string Diagnosis { get; set; }

        public string Notes { get; set; }

        public DateTime VisitDate { get; set; }
    }
}