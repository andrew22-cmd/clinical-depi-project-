using Microsoft.AspNetCore.Identity;

namespace clinicsystem.Models
{
  
    public class User : IdentityUser
    {
   
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

   
        public string Role { get; set; } = "Patient";

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        // ── Navigation properties ────────────────────────────────────────────
     
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}