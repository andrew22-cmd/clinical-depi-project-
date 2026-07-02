using clinicsystem.Models;
using System.ComponentModel.DataAnnotations;

namespace clinicsystem.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }  // null on Create, populated on Edit/Details

        [Required(ErrorMessage = "الاسم الأول مطلوب.")]
        [Display(Name = "الاسم الأول")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم العائلة مطلوب.")]
        [Display(Name = "اسم العائلة")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        // Computed — replaces UserProfileViewModel.FullName
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Factory — replaces UserProfileViewModel.FromUser()
        public static UserViewModel FromUser(clinicsystem.Models.User? user)
        {
            return new UserViewModel
            {
                Id = user?.Id,
                FirstName = user?.FirstName ?? string.Empty,
                LastName = user?.LastName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                Phone = user?.PhoneNumber ?? string.Empty,
                Role = (user?.Role ?? "Patient").ToLowerInvariant() == "patient"
                            ? "Patient" : user?.Role ?? "Patient"
            };
        }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة.")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "رقم الهاتف")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "الدور مطلوب.")]
        [Display(Name = "الدور")]
        public string Role { get; set; } = "Patient";

        // Password required only on Create; optional on Edit
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        [StringLength(40, MinimumLength = 8,
            ErrorMessage = "كلمة المرور يجب أن تكون بين 8 و 40 حرفاً.")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين.")]
        public string? ConfirmPassword { get; set; }
    }

   
    public class UserIndexViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; } = new List<UserViewModel>();

        // Search & filter
        public string? SearchTerm { get; set; }
        public string? FilterRole { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

    }
}
