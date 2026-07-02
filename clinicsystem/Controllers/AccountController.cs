
using clinicsystem.Data;
using clinicsystem.Models;
using clinicsystem.Services;
using clinicsystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace clinicsystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _db;
        private readonly Services.IEmailSender _emailSender;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            AppDbContext db,
            Services.IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            _emailSender = emailSender;
        }

        // ── Helper: build + send a confirmation email for a given user ────────
        private async Task SendConfirmationEmailAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = Url.Action(
                action: "ConfirmEmail",
                controller: "Account",
                values: new { userId = user.Id, token = encodedToken },
                protocol: Request.Scheme)!;

            var body = $@"
                <div style=""font-family:Tahoma,Arial,sans-serif;line-height:1.8;"">
                    <h2>تأكيد البريد الإلكتروني</h2>
                    <p>مرحباً {user.FullName}،</p>
                    <p>شكراً لتسجيلك في نظام العيادة. يرجى الضغط على الرابط التالي لتفعيل حسابك:</p>
                    <p><a href=""{confirmationLink}"">تفعيل الحساب</a></p>
                    <p>إذا لم تقم بإنشاء هذا الحساب، يمكنك تجاهل هذه الرسالة.</p>
                </div>";

            await _emailSender.SendEmailAsync(user.Email!, "تأكيد البريد الإلكتروني - نظام العيادة", body);
        }

        // ── GET /Account/Login ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel());
        }

        // ── POST /Account/Login ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                userName: model.Email,
                password: model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: false);

            if (result.IsNotAllowed)
            {
                // Password was correct, but SignInManager blocked the sign-in because
                // RequireConfirmedEmail is enabled and the account isn't confirmed yet.
                ModelState.AddModelError(string.Empty,
                    "يجب تأكيد البريد الإلكتروني قبل تسجيل الدخول. لم تستلم رسالة التأكيد؟");
                ViewBag.UnconfirmedEmail = model.Email;
                return View(model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty,
                    "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                return View(model);
            }

            // Identity cookie is now set — user data flows through
            // HttpContext.User / UserManager on every subsequent request.
            // No TempData or localStorage needed.
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("Index", "Home");

            return user.Role switch
            {
                UserRoles.Manager => RedirectToAction("Dashboard", "Manager"),
                UserRoles.Doctor => RedirectToAction("Dashboard", "Doctor"),
                UserRoles.Secretary => RedirectToAction("Dashboard", "Secretary"),
                _ => RedirectToAction("Dashboard", "Patient")
            };
        }

        // ── GET /Account/Register ────────────────────────────────────────────
        [HttpGet]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel());
        }

        // ── POST /Account/Register ───────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1. Create the Identity user — password is hashed by Identity.
            //    EmailConfirmed stays false (IdentityUser default) — the account
            //    is only activated once the user clicks the emailed link.
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone,
                Role = UserRoles.Patient
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            // 2. Assign the Identity role.
            await _userManager.AddToRoleAsync(user, UserRoles.Patient);

            // 3. Create the linked Patient record in the Patients table.
            //    This is the single source of truth — no localStorage involved.
            var patient = new Patient
            {
                FullName = user.FullName,
                Phone = user.PhoneNumber ?? string.Empty,
                CreatedAt = DateTime.UtcNow
                // UserId left null — Patient.UserId is int, User.Id is string.
                // Link is maintained by querying Patient by email/name when needed.
            };
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            // 4. Generate and email the confirmation link. The user is NOT signed in
            //    here — RequireConfirmedAccount/RequireConfirmedEmail means they must
            //    confirm first, so we send them to a "check your email" page instead.
            try
            {
                await SendConfirmationEmailAsync(user);
            }
            catch (Exception)
            {
                // Account exists but the email failed to send — let the user know
                // so they aren't left stuck with no way to activate their account.
                TempData["RegisterConfirmationMessage"] =
                    "تم إنشاء حسابك، لكن تعذر إرسال رسالة التأكيد. يمكنك طلب إرسال رابط جديد لاحقاً.";
                return RedirectToAction(nameof(RegisterConfirmation), new { email = user.Email });
            }

            TempData["RegisterConfirmationMessage"] =
                "تم إنشاء حسابك بنجاح! تم إرسال رابط تأكيد إلى بريدك الإلكتروني، يرجى الضغط عليه لتفعيل الحساب.";
            return RedirectToAction(nameof(RegisterConfirmation), new { email = user.Email });
        }

        // ── GET /Account/RegisterConfirmation ────────────────────────────────
        // Shown right after registration (and reused after a resend request).
        [HttpGet]
        public IActionResult RegisterConfirmation(string? email)
        {
            ViewBag.Email = email;
            ViewBag.Message = TempData["RegisterConfirmationMessage"] as string
                ?? "تم إرسال رابط تأكيد إلى بريدك الإلكتروني.";
            return View();
        }

        // ── GET /Account/ConfirmEmail ─────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return View(new ConfirmEmailViewModel
                {
                    Succeeded = false,
                    Message = "رابط التأكيد غير صالح."
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View(new ConfirmEmailViewModel
                {
                    Succeeded = false,
                    Message = "لا يوجد حساب مرتبط بهذا الرابط."
                });
            }

            if (user.EmailConfirmed)
            {
                return View(new ConfirmEmailViewModel
                {
                    Succeeded = true,
                    Message = "تم تأكيد بريدك الإلكتروني مسبقاً. يمكنك تسجيل الدخول الآن."
                });
            }

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch (FormatException)
            {
                return View(new ConfirmEmailViewModel
                {
                    Succeeded = false,
                    Message = "رابط التأكيد غير صالح.",
                    Email = user.Email
                });
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                return View(new ConfirmEmailViewModel
                {
                    Succeeded = true,
                    Message = "تم تأكيد بريدك الإلكتروني بنجاح! يمكنك تسجيل الدخول الآن."
                });
            }

            // Token invalid, expired, or already used — offer a way to get a new one.
            return View(new ConfirmEmailViewModel
            {
                Succeeded = false,
                Message = "رابط التأكيد غير صالح أو منتهي الصلاحية. يرجى طلب رابط جديد.",
                Email = user.Email
            });
        }

        // ── GET /Account/ResendEmailConfirmation ─────────────────────────────
        [HttpGet]
        public IActionResult ResendEmailConfirmation(string? email)
        {
            return View(new ResendEmailConfirmationViewModel { Email = email ?? string.Empty });
        }

        // ── POST /Account/ResendEmailConfirmation ────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            // Don't reveal whether the account exists or is already confirmed —
            // always show the same generic confirmation message.
            if (user != null && !user.EmailConfirmed)
            {
                try
                {
                    await SendConfirmationEmailAsync(user);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "تعذر إرسال رسالة التأكيد حالياً، يرجى المحاولة لاحقاً.");
                    return View(model);
                }
            }

            TempData["RegisterConfirmationMessage"] =
                "إذا كان بريدك الإلكتروني مسجلاً وغير مؤكد، فقد تم إرسال رابط تأكيد جديد إليه.";
            return RedirectToAction(nameof(RegisterConfirmation), new { email = model.Email });
        }

        // ── GET /Account/Logout ──────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ── GET /Account/ForgotPassword ──────────────────────────────────────
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        // ── POST /Account/ForgotPassword ─────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            // Never reveal whether the email exists in the system.
            if (user == null)
                return RedirectToAction(nameof(ForgotPasswordConfirmation));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(
                action: "ResetPassword",
                controller: "Account",
                values: new { token, email = model.Email },
                protocol: Request.Scheme)!;

            // No email service configured — show link directly on confirmation page.
            TempData["ResetLink"] = resetLink;

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        // ── GET /Account/ForgotPasswordConfirmation ──────────────────────────
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // ── GET /Account/ResetPassword ───────────────────────────────────────
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return BadRequest("رابط إعادة تعيين كلمة المرور غير صالح.");

            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        // ── POST /Account/ResetPassword ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction(nameof(ResetPasswordConfirmation));

            var result = await _userManager.ResetPasswordAsync(
                user, model.Token, model.NewPassword);

            if (result.Succeeded)
                return RedirectToAction(nameof(ResetPasswordConfirmation));

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ── GET /Account/ResetPasswordConfirmation ───────────────────────────
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}