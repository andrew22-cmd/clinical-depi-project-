using clinicsystem.Models;
using clinicsystem.ViewModels;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();

    Task<User?> GetUserByIdAsync(int id);

    Task<User?> LoginAsync(string email, string password);

    Task<bool> RegisterAsync(User user);

    Task<User?> RegisterPatientAsync(RegisterViewModel model);

    Task<bool> UpdateUserAsync(User user);

    Task<bool> DeleteUserAsync(int id);

    Task<bool> EmailExistsAsync(string email);

    Task<bool> PhoneExistsAsync(string phone);
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByConfirmationTokenAsync(string token);
    Task GenerateEmailConfirmationTokenAsync(User user);

    Task GenerateResetPasswordTokenAsync(User user);

    Task<bool> ResetPasswordAsync(string token, string newPassword);

}