using System.Numerics;

namespace clinicsystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public bool EmailConfirmed { get; set; }

        public string? EmailConfirmationToken { get; set; }

        public DateTime? EmailConfirmationTokenExpiry { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string Password { get; set; }
        public string Role { get; set; }
        public string? ResetPasswordToken { get; set; }

        public DateTime? ResetPasswordTokenExpiry { get; set; }


        // Navigation
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}
