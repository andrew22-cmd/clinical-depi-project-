using clinicsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ClinicDbContext context)
        {
            await context.Database.MigrateAsync();

            if (await context.Users.AnyAsync())
                return;

            #region Users

            var users = new List<User>
            {
                // Manager
                new() { FirstName="System",  LastName="Manager",   Email="manager@gmail.com",    Phone="01000000001", Password="123456", Role="Manager" },
                // Secretary
                new() { FirstName="آمال",    LastName="يوسف",     Email="secretary@gmail.com",  Phone="01000000002", Password="123456", Role="Secretary" },
                // Doctors (indices 2-6)
                new() { FirstName="أحمد",    LastName="الشريف",   Email="doctor1@gmail.com",    Phone="01100000001", Password="123456", Role="Doctor" },
                new() { FirstName="محمد",    LastName="حسان",     Email="doctor2@gmail.com",    Phone="01100000002", Password="123456", Role="Doctor" },
                new() { FirstName="عمر",     LastName="محمود",    Email="doctor3@gmail.com",    Phone="01100000003", Password="123456", Role="Doctor" },
                new() { FirstName="ريم",     LastName="السيد",    Email="doctor4@gmail.com",    Phone="01100000004", Password="123456", Role="Doctor" },
                new() { FirstName="خالد",    LastName="إبراهيم",  Email="doctor5@gmail.com",    Phone="01100000005", Password="123456", Role="Doctor" },
                // Patients (indices 7-11)
                new() { FirstName="علي",     LastName="المنصور",  Email="patient1@gmail.com",   Phone="01200000001", Password="123456", Role="Patient" },
                new() { FirstName="منى",     LastName="الصادق",   Email="patient2@gmail.com",   Phone="01200000002", Password="123456", Role="Patient" },
                new() { FirstName="نور",     LastName="حلمي",     Email="patient3@gmail.com",   Phone="01200000003", Password="123456", Role="Patient" },
                new() { FirstName="كريم",    LastName="عبد الله", Email="patient4@gmail.com",   Phone="01200000004", Password="123456", Role="Patient" },
                new() { FirstName="سمر",     LastName="النجار",   Email="patient5@gmail.com",   Phone="01200000005", Password="123456", Role="Patient" },
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            #endregion

            #region Specialities

            var specialities = new List<Speciality>
            {
                new(){ Name="طب الأسنان",       ConsultationFee = 150m },
                new(){ Name="أمراض القلب",      ConsultationFee = 250m },
                new(){ Name="جراحة العظام",     ConsultationFee = 200m },
                new(){ Name="الطب الباطني",     ConsultationFee = 150m },
                new(){ Name="الجلدية والتجميل", ConsultationFee = 180m },
            };

            await context.Specialities.AddRangeAsync(specialities);
            await context.SaveChangesAsync();

            #endregion

            #region Doctors

            var doctors = new List<Doctor>
            {
                new() { UserId=users[2].UserId,  SpecialityId=specialities[0].SpecialityId }, // أسنان
                new() { UserId=users[3].UserId,  SpecialityId=specialities[1].SpecialityId }, // قلب
                new() { UserId=users[4].UserId,  SpecialityId=specialities[2].SpecialityId }, // عظام
                new() { UserId=users[5].UserId,  SpecialityId=specialities[3].SpecialityId }, // باطنة
                new() { UserId=users[6].UserId,  SpecialityId=specialities[4].SpecialityId }, // جلدية
            };

            await context.Doctors.AddRangeAsync(doctors);
            await context.SaveChangesAsync();

            #endregion

            #region Patients

            var patients = new List<Patient>
            {
                new() { UserId=users[7].UserId,  FullName="علي المنصور",   Phone="01200000001", CreatedAt=DateTime.Now },
                new() { UserId=users[8].UserId,  FullName="منى الصادق",    Phone="01200000002", CreatedAt=DateTime.Now },
                new() { UserId=users[9].UserId,  FullName="نور حلمي",      Phone="01200000003", CreatedAt=DateTime.Now },
                new() { UserId=users[10].UserId, FullName="كريم عبد الله", Phone="01200000004", CreatedAt=DateTime.Now },
                new() { UserId=users[11].UserId, FullName="سمر النجار",    Phone="01200000005", CreatedAt=DateTime.Now },
            };

            await context.Patients.AddRangeAsync(patients);
            await context.SaveChangesAsync();

            #endregion
            #region DoctorSchedules

            var schedules = new List<DoctorSchedule>();

            string[] days =
            {
                "السبت",
                "الأحد",
                "الاثنين",
                "الثلاثاء",
                "الأربعاء"
            };

            foreach (var doctor in doctors)
            {
                foreach (var day in days)
                {
                    schedules.Add(new DoctorSchedule
                    {
                        DoctorId = doctor.DoctorId,
                        WeekDay = day,
                        IsActive = true
                    });
                }
            }

            await context.DoctorSchedules.AddRangeAsync(schedules);
            await context.SaveChangesAsync();

            #endregion
            #region DoctorScheduleSlots

            var slots = new List<DoctorScheduleSlot>();

            TimeSpan start = new TimeSpan(9, 0, 0);

            foreach (var schedule in schedules)
            {
                for (int i = 0; i < 8; i++)
                {
                    slots.Add(new DoctorScheduleSlot
                    {
                        ScheduleId = schedule.ScheduleId,
                        SlotTime = start.Add(TimeSpan.FromMinutes(i * 30)),
                        IsBooked = false,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            await context.DoctorScheduleSlots.AddRangeAsync(slots);
            await context.SaveChangesAsync();

            #endregion
        }
    }
}