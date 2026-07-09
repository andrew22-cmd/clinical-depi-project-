using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Models
{
    public class ClinicDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<DoctorScheduleSlot> DoctorScheduleSlots { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<PatientNotes> PatientNotes { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }

        public ClinicDbContext(DbContextOptions<ClinicDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // مثال important
            modelBuilder.Entity<User>()
                .HasOne(u => u.Doctor)
                .WithOne(d => d.User)
                .HasForeignKey<Doctor>(d => d.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Patient)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserId);

            // Configure Primary Keys for entities that do not follow the default naming convention
            modelBuilder.Entity<DoctorSchedule>().HasKey(d => d.ScheduleId);
            modelBuilder.Entity<DoctorScheduleSlot>().HasKey(d => d.SlotId);
            modelBuilder.Entity<PatientNotes>().HasKey(p => p.NoteId);
            modelBuilder.Entity<EmailNotification>().HasKey(e => e.NotificationId);

            // Configure Delete Behaviors to avoid cycles or multiple cascade paths
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.Reservations)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Patient)
                .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
