using clinicsystem.Models;

namespace clinicsystem.ViewModels
{
	public class ReservationViewModel
	{
		public int? SelectedSpecialityId { get; set; }

		public int? SelectedDoctorId { get; set; }

		public int? SelectedScheduleId { get; set; }

		public int? SelectedSlotId { get; set; }

		public IEnumerable<Speciality> Specialities { get; set; } = new List<Speciality>();

		public IEnumerable<Doctor> Doctors { get; set; } = new List<Doctor>();

		public IEnumerable<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();

		public IEnumerable<DoctorScheduleSlot> Slots { get; set; } = new List<DoctorScheduleSlot>();
	}
}