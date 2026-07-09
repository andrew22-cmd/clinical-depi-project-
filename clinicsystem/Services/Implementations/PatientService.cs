using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using clinicsystem.Services.Interfaces;

namespace clinicsystem.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _patientRepository.GetAllAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _patientRepository.GetByIdAsync(id);
        }

        public async Task<Patient?> GetByUserIdAsync(int userId)
        {
            return await _patientRepository.GetByUserIdAsync(userId);
        }

        public async Task<Patient?> GetByPhoneAsync(string phone)
        {
            return await _patientRepository.GetByPhoneAsync(phone);
        }

        public async Task<Patient?> GetWithReservationsAsync(int patientId)
        {
            return await _patientRepository.GetWithReservationsAsync(patientId);
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string keyword)
        {
            return await _patientRepository.SearchAsync(keyword);
        }

        public async Task AddPatientAsync(Patient patient)
        {
            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveAsync();
        }

        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            var existing = await _patientRepository.GetByIdAsync(patient.PatientId);

            if (existing == null)
                return false;

            existing.FullName = patient.FullName;
            existing.Phone = patient.Phone;
            existing.UserId = patient.UserId;

            _patientRepository.Update(existing);

            await _patientRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);

            if (patient == null)
                return false;

            _patientRepository.Delete(patient);

            await _patientRepository.SaveAsync();

            return true;
        }
        public async Task<Patient> CreateOfflinePatientAsync(string name, string phone)
        {
            var patient = new Patient
            {
                UserId = null,
                FullName = name,
                Phone = phone,
                CreatedAt = DateTime.Now
            };

            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveAsync();

            return patient;
        }
    }
}