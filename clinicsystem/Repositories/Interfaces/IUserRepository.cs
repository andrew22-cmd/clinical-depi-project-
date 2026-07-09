using clinicsystem.Models;

namespace clinicsystem.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByPhoneAsync(string phone);

        Task<User?> LoginAsync(string email, string password);

        Task<IEnumerable<User>> GetByRoleAsync(string role);

        Task<bool> EmailExistsAsync(string email);

        Task<bool> PhoneExistsAsync(string phone);
  

        Task<User?> GetByConfirmationTokenAsync(string token);
        Task<User?> GetByResetPasswordTokenAsync(string token);
        
        Task SaveAsync();
    }
}