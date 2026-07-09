using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using clinicsystem.Services.Email;
using clinicsystem.Services.Interfaces;

namespace clinicsystem.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorScheduleSlotRepository _slotRepository;
        private readonly IEmailNotificationRepository _emailRepository;
        private readonly IEmailSender _emailSender;

        public ReservationService(
            IReservationRepository reservationRepository,
            IPatientRepository patientRepository,
            IDoctorScheduleSlotRepository slotRepository,
            IEmailNotificationRepository emailRepository,IEmailSender emailSender)
        {
            _reservationRepository = reservationRepository;
            _patientRepository = patientRepository;
            _slotRepository = slotRepository;
            _emailRepository = emailRepository;
            _emailSender = emailSender;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _reservationRepository.GetAllWithDetailsAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _reservationRepository.GetReservationWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Reservation>> GetTodayReservationsAsync()
        {
            return await _reservationRepository.GetTodayReservationsAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByDoctorAsync(int doctorId)
        {
            return await _reservationRepository.GetByDoctorAsync(doctorId);
        }

        public async Task<IEnumerable<Reservation>> GetByPatientAsync(int patientId)
        {
            return await _reservationRepository.GetByPatientAsync(patientId);
        }

        public async Task<IEnumerable<Reservation>> GetByStatusAsync(string status)
        {
            return await _reservationRepository.GetByStatusAsync(status);
        }

        public async Task<bool> CreateReservationAsync(Reservation reservation)
        {
            var slot = await _slotRepository.GetByIdAsync(reservation.SlotId);

            if (slot == null)
                return false;

            if (slot.IsBooked)
                return false;

            reservation.CreatedAt = DateTime.Now;

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveAsync();
            var reservationDetails =
    await _reservationRepository
        .GetReservationWithDetailsAsync(reservation.ReservationId);

            // Only send confirmation email for online patients (who have a registered account/email)
            if (reservationDetails != null && reservationDetails.Patient?.User?.Email != null)
            {
                try
                {
                    await _emailSender.SendEmailAsync(
                        reservationDetails.Patient.User.Email,
                        "Reservation Confirmation",
                        $@"
<h2>Reservation Confirmed ✅</h2>

<p>Hello {reservationDetails.Patient.FullName},</p>

<p>Your reservation has been successfully booked.</p>

<hr>

<p><b>Doctor:</b> Dr. {reservationDetails.Doctor.User.FirstName} {reservationDetails.Doctor.User.LastName}</p>

<p><b>Date:</b> {reservationDetails.ReservationDate:dd/MM/yyyy}</p>

<p><b>Day:</b> {reservationDetails.Slot.Schedule.WeekDay}</p>

<p><b>Time:</b> {reservationDetails.Slot.SlotTime:hh\:mm}</p>

<p><b>Status:</b> {reservationDetails.Status}</p>

<hr>

<p>Thank you for using Clinic System ❤️</p>
");
                }
                catch
                {
                    // Email failure should not block reservation creation
                }
            }
            var notification = new EmailNotification
            {
                ReservationId = reservation.ReservationId,
                RecipientUserId = reservation.CreatedBy ?? 0,
                Channel = "Email",
                Status = "Pending",
                CreatedAt = DateTime.Now,
                SentAt = DateTime.Now
            };

            await _emailRepository.AddAsync(notification);
            await _emailRepository.SaveAsync();

            return true;
        }

        public async Task<bool> UpdateReservationAsync(Reservation reservation)
        {
            var existing = await _reservationRepository.GetByIdAsync(reservation.ReservationId);

            if (existing == null)
                return false;

            existing.PatientId = reservation.PatientId;
            existing.DoctorId = reservation.DoctorId;
            existing.SlotId = reservation.SlotId;
            existing.ReservationDate = reservation.ReservationDate;
            existing.Status = reservation.Status;
            existing.Source = reservation.Source;
            existing.CreatedBy = reservation.CreatedBy;

            _reservationRepository.Update(existing);
            await _reservationRepository.SaveAsync();

            return true;
        }

        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
                return false;

            var slot = await _slotRepository.GetByIdAsync(reservation.SlotId);

           

            _reservationRepository.Delete(reservation);
            await _reservationRepository.SaveAsync();

            return true;
        }
        public async Task<int> GetReservationsCountAsync(int patientId)
        {
            return await _reservationRepository.GetReservationsCountAsync(patientId);
        }
        public async Task<IEnumerable<Reservation>> GetPatientReservationsDetailsAsync(int patientId)
        {
            return await _reservationRepository.GetPatientReservationsDetailsAsync(patientId);
        }
        public async Task<IEnumerable<Reservation>> GetDoctorTodayReservationsAsync(int doctorId)
        {
            return await _reservationRepository.GetDoctorTodayReservationsAsync(doctorId);
        }
        public async Task<IEnumerable<Reservation>> GetDoctorReservationsForNotesAsync(int doctorId)
        {
            return await _reservationRepository.GetDoctorReservationsForNotesAsync(doctorId);
        }
        public async Task<Reservation?> GetReservationWithDetailsAsync(int reservationId)
        {
            return await _reservationRepository
                .GetReservationWithDetailsAsync(reservationId);
        }
        public async Task<IEnumerable<Reservation>> GetByDateAsync(DateTime date)
        {
            return await _reservationRepository.GetByDateAsync(date);
        }

        public Task<bool> IsSlotBookedAsync(int slotId, DateTime reservationDate)
        {
            throw new NotImplementedException();
        }

    }
}