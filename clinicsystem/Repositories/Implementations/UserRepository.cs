using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ClinicDbContext context)
            : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Phone == phone);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    u.Password == password);
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Users
                .AnyAsync(u => u.Phone == phone);
        }
       

        public async Task<User?> GetByConfirmationTokenAsync(string token)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.EmailConfirmationToken == token);
        }
        public async Task<User?> GetByResetPasswordTokenAsync(string token)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.ResetPasswordToken == token);
        }
    }
}