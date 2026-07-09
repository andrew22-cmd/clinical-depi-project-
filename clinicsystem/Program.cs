using clinicsystem.Models;
using clinicsystem.Repositories.Implementations;
using clinicsystem.Repositories.Interfaces;
//using clinicsystem.Repository;
using clinicsystem.Services.Implementations;
using clinicsystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using clinicsystem.Data;
using clinicsystem.Services.Email;

namespace clinicsystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ================================
            // Add MVC
            // ================================
            builder.Services.AddControllersWithViews()
      .AddJsonOptions(options =>
      {
          // ????? ?? ????? ??? JSON encoder ???? ?????? ??????? ?????? ?????? ???????
          options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
      });

            // ================================
            // Arabic Encoding
            // ================================
            builder.Services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            builder.Services.AddSingleton<HtmlEncoder>(
                HtmlEncoder.Create(new[]
                {
                    UnicodeRanges.BasicLatin,
                    UnicodeRanges.Arabic
                }));

            // ================================
            // Database
            // ================================
            builder.Services.AddDbContext<ClinicDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("constring")));

            // ================================
            // Generic Repository
            // ================================
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // ================================
            // Repositories
            // ================================
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
            builder.Services.AddScoped<IDoctorScheduleSlotRepository, DoctorScheduleSlotRepository>();
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IPatientNotesRepository, PatientNotesRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<ISpecialityRepository, SpecialityRepository>();
            builder.Services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IDoctorScheduleSlotService, DoctorScheduleSlotService>();
            builder.Services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IPatientNotesService, PatientNotesService>();
            builder.Services.AddScoped<ISpecialityService, SpecialityService>();
            builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();

            builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddScoped<IEmailSender, EmailService>();


            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<ClinicDbContext>();

                await DbSeeder.SeedAsync(context);
            }

            // ================================
            // Configure Pipeline
            // ================================
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // app.UseHttpsRedirection(); // ????? ??? ????? ??? SSL

            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Home}/{id?}");

            app.Run();
        }
    }
}

//bjuc yezs oggl vst