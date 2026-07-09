using clinicsystem.Models;
using clinicsystem.Repositories.Implementations;
using clinicsystem.Repositories.Interfaces;
using clinicsystem.Services.Interfaces;
using clinicsystem.ViewModels;

namespace clinicsystem.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatientRepository _patientRepository;

        public UserService(
            IUserRepository userRepository,
            IPatientRepository patientRepository)
        {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            if (string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return null;

            if (user.Password != password)
                return null;

            return user;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            if (await _userRepository.EmailExistsAsync(user.Email))
                return false;

            if (await _userRepository.PhoneExistsAsync(user.Phone))
                return false;

            await _userRepository.AddAsync(user);

            await _userRepository.SaveAsync();

            return true;
        }
        public async Task<User?> RegisterPatientAsync(RegisterViewModel model)
        {
            if (await _userRepository.EmailExistsAsync(model.Email))
                return null;

            if (await _userRepository.PhoneExistsAsync(model.Phone))
                return null;

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Password = model.Password,
                Role = "Patient",

                EmailConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString(),
                EmailConfirmationTokenExpiry = DateTime.Now.AddHours(24)
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            var patient = new Patient
            {
                UserId = user.UserId,
                FullName = $"{model.FirstName} {model.LastName}",
                Phone = model.Phone,
                CreatedAt = DateTime.Now
            };

            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveAsync();

            return user;
        }
        public async Task<bool> UpdateUserAsync(User user)
        {
            var existing = await _userRepository.GetByIdAsync(user.UserId);

            if (existing == null)
                return false;

            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.Email = user.Email;
            existing.Phone = user.Phone;
            existing.Password = user.Password;
            existing.Role = user.Role;

            _userRepository.Update(existing);

            await _userRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return false;

            _userRepository.Delete(user);

            await _userRepository.SaveAsync();

            return true;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _userRepository.PhoneExistsAsync(phone);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User?> GetByConfirmationTokenAsync(string token)
        {
            return await _userRepository.GetByConfirmationTokenAsync(token);
        }
        public async Task GenerateEmailConfirmationTokenAsync(User user)
        {
            user.EmailConfirmationToken = Guid.NewGuid().ToString();

            user.EmailConfirmationTokenExpiry =
                DateTime.Now.AddHours(24);

            _userRepository.Update(user);

            await _userRepository.SaveAsync();
        }
        public async Task GenerateResetPasswordTokenAsync(User user)
        {
            user.ResetPasswordToken = Guid.NewGuid().ToString();

            user.ResetPasswordTokenExpiry = DateTime.Now.AddHours(1);

            _userRepository.Update(user);

            await _userRepository.SaveAsync();
        }
        public async Task<bool> ResetPasswordAsync(
    string token,
    string newPassword)
        {
            var user =
                await _userRepository.GetByResetPasswordTokenAsync(token);

            if (user == null)
                return false;

            if (user.ResetPasswordTokenExpiry < DateTime.Now)
                return false;

            user.Password = newPassword;

            user.ResetPasswordToken = null;

            user.ResetPasswordTokenExpiry = null;

            _userRepository.Update(user);

            await _userRepository.SaveAsync();

            return true;
        }

    }

}