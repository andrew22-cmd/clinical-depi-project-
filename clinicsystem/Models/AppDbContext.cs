using clinicsystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Data
{
   
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ── Clinic domain tables ─────────────────────────────────────────────
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<DoctorScheduleSlot> DoctorScheduleSlots { get; set; }
        public DbSet<PatientNotes> PatientNotes { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // MUST call base first — registers all Identity table configurations.
            base.OnModelCreating(modelBuilder);

            // ── Doctor ───────────────────────────────────────────────────────
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(d => d.DoctorId);

                entity.HasOne(d => d.Speciality)
                      .WithMany(s => s.Doctors)
                      .HasForeignKey(d => d.SpecialityId);

                entity.HasMany(d => d.Schedules)
                      .WithOne(s => s.Doctor)
                      .HasForeignKey(s => s.DoctorId);

                entity.HasMany(d => d.Reservations)
                      .WithOne(r => r.Doctor)
                      .HasForeignKey(r => r.DoctorId).OnDelete(DeleteBehavior.Restrict); ;

                // Ignore navigation to Users — Doctor.UserId is int,
                // IdentityUser.Id is string. Type mismatch prevents FK mapping.
                entity.Ignore(d => d.User);
            });

            // ── Patient ──────────────────────────────────────────────────────
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.PatientId);

                entity.HasMany(p => p.Reservations)
                      .WithOne(r => r.Patient)
                      .HasForeignKey(r => r.PatientId).OnDelete(DeleteBehavior.Restrict); ;

                // Same int↔string issue — ignore the Users navigation.
                entity.Ignore(p => p.User);
            });

            // ── Reservation ──────────────────────────────────────────────────
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(r => r.ReservationId);

                entity.HasOne(r => r.Slot)
                      .WithMany(s => s.Reservations)
                      .HasForeignKey(r => r.SlotId);

                entity.HasMany(r => r.Notes)
                      .WithOne(n => n.Reservation)
                      .HasForeignKey(n => n.ReservationId);

                entity.HasMany(r => r.Notifications)
                      .WithOne(n => n.Reservation)
                      .HasForeignKey(n => n.ReservationId);
            });

            // ── DoctorSchedule ───────────────────────────────────────────────
            modelBuilder.Entity<DoctorSchedule>(entity =>
            {
                entity.HasKey(s => s.ScheduleId);

                entity.HasMany(s => s.Slots)
                      .WithOne(sl => sl.Schedule)
                      .HasForeignKey(sl => sl.ScheduleId);
            });

            // ── DoctorScheduleSlot ───────────────────────────────────────────
            modelBuilder.Entity<DoctorScheduleSlot>(entity =>
            {
                entity.HasKey(s => s.SlotId);
            });

            // ── PatientNotes ─────────────────────────────────────────────────
            modelBuilder.Entity<PatientNotes>(entity =>
            {
                entity.HasKey(n => n.NoteId);
            });

            // ── EmailNotification ────────────────────────────────────────────
            modelBuilder.Entity<EmailNotification>(entity =>
            {
                entity.HasKey(n => n.NotificationId);
            });

            // ── Speciality ───────────────────────────────────────────────────
            modelBuilder.Entity<Speciality>(entity =>
            {
                entity.HasKey(s => s.SpecialityId);
            });
        }
    }
}