using clinicsystem.Models;
using clinicsystem.Services.Email;
using clinicsystem.Services.Interfaces;
using clinicsystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using clinicsystem.Services.Email;

namespace clinicsystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;  
        

        public AccountController(
            IUserService userService,
            IEmailSender emailSender)
        {
            _userService = userService;
            _emailSender = emailSender;
        }
        
        [HttpGet]
        public IActionResult ResendEmailConfirmation(string? email)
        {
            return View(new ResendEmailViewModel
            {
                Email = email
            });
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        #region Login

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "Please correct the validation errors.";
                return View(model);
            }

            var user = await _userService.LoginAsync(model.Email, model.Password);

            if (user == null)
            {
                TempData["Error"] = "Login failed. Invalid Email or Password.";
                ModelState.AddModelError("", "Invalid Email or Password.");
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                TempData["Warning"] = "Login failed. Please confirm your email first.";
                ModelState.AddModelError("", "Please confirm your email before logging in. Check your inbox or resend the confirmation email.");
                ViewBag.UnconfirmedEmail = user.Email;
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
            HttpContext.Session.SetString("Role", user.Role);

            TempData["Success"] = "Login successful. Welcome!";

            switch (user.Role)
            {
                case "Manager":
                    return RedirectToAction("DashBoard", "Manager");

                case "Secretary":
                    return RedirectToAction("DashBoard", "Secretary");

                case "Doctor":
                    return RedirectToAction("DashBoard", "Doctor");

                case "Patient":
                    return RedirectToAction("DashBoard", "Patient");

                default:
                    return RedirectToAction("Home", "Home");
            }
        }
        //[HttpGet]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(
    ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "Please correct the validation errors.";
                return View(model);
            }

            var user = await _userService.GetByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["Success"] =
                    "If the email exists, a reset link has been sent.";

                return RedirectToAction(nameof(Login));
            }

            await _userService.GenerateResetPasswordTokenAsync(user);

            var link = Url.Action(
                "ResetPassword",
                "Account",
                new
                {
                    token = user.ResetPasswordToken,
                    email = user.Email
                },
                Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset Password",
                $@"
<h2>Clinic System</h2>

<p>Click below to reset your password.</p>

<a href='{link}'
style='padding:10px 20px;
background:#0d6efd;
color:white;
text-decoration:none'>
Reset Password
</a>");

            TempData["Success"] =
                "Reset link sent successfully.";

            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "Please correct the validation errors.";
                return View(model);
            }

            var user = await _userService.RegisterPatientAsync(model);

            if (user == null)
            {
                TempData["Error"] = "Registration failed. Email or Phone already exists.";
                ModelState.AddModelError("", "Email or Phone already exists.");
                return View(model);
            }

            var link = Url.Action(
                "ConfirmEmail",
                "Account",
                new
                {
                    token = user.EmailConfirmationToken
                },
                Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Confirm Your Email",
                $@"
        <h2>Welcome to Clinic System</h2>

        <p>Please click the button below to confirm your email.</p>

        <a href='{link}'
           style='padding:12px 20px;
                  background:#0d6efd;
                  color:white;
                  text-decoration:none;
                  border-radius:5px'>
            Confirm Email
        </a>");

            ViewBag.Email = user.Email;

            TempData["Success"] = "Registration successful. Email confirmation sent.";

            return View("EmailConfirmationSent");
        }
        #endregion

        #region Logout

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Logout successful.";
            return RedirectToAction(nameof(Login));
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var model = new ConfirmEmailViewModel();

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Email confirmation failed. Invalid token.";
                model.Succeeded = false;
                model.Message = "رابط التأكيد غير صالح.";

                return View(model);
            }

            var user =
                await _userService.GetByConfirmationTokenAsync(token);

            if (user == null)
            {
                TempData["Error"] = "Email confirmation failed. User not found.";
                model.Succeeded = false;
                model.Message = "رابط التأكيد غير صحيح.";

                return View(model);
            }

            if (user.EmailConfirmationTokenExpiry < DateTime.Now)
            {
                TempData["Warning"] = "Email confirmation failed. Link expired.";
                model.Succeeded = false;
                model.Email = user.Email;
                model.Message = "انتهت صلاحية رابط التأكيد.";

                return View(model);
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiry = null;

            await _userService.UpdateUserAsync(user);

            TempData["Success"] = "Email confirmed successfully.";

            model.Succeeded = true;
            model.Message = "تم تأكيد البريد الإلكتروني بنجاح، يمكنك الآن تسجيل الدخول.";

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(
    ResendEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "Please provide a valid email.";
                return View(model);
            }

            var user = await _userService.GetByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["Error"] = "Resend failed. Email not found.";
                ModelState.AddModelError("", "Email not found");
                return View(model);
            }

            if (user.EmailConfirmed)
            {
                TempData["Info"] = "Email is already confirmed.";
                ModelState.AddModelError("", "Email already confirmed");
                return View(model);
            }

            await _userService.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action(
                "ConfirmEmail",
                "Account",
                new
                {
                    token = user.EmailConfirmationToken
                },
                Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Confirm Email",
                $@"
        <h2>Confirm your email</h2>

        <a href='{link}'>
            Confirm
        </a>");

            TempData["Success"] =
                "Confirmation email sent.";

            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(
    string token,
    string email)
        {
            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Invalid reset link.";
                return View("InvalidResetPassword");
            }

            var user = await _userService.GetByEmailAsync(email);

            if (user == null)
            {
                TempData["Error"] = "Invalid reset link.";
                return View("InvalidResetPassword");
            }

            if (user.ResetPasswordToken != token)
            {
                TempData["Error"] = "Invalid reset link.";
                return View("InvalidResetPassword");
            }

            if (user.ResetPasswordTokenExpiry < DateTime.Now)
            {
                TempData["Warning"] = "Reset link has expired.";
                return View("InvalidResetPassword");
            }

            return View(new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(
    ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "Please correct the validation errors.";
                return View(model);
            }

            var user = await _userService.GetByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["Error"] = "Invalid reset link (user not found).";
                return View("InvalidResetPassword");
            }

            if (user.ResetPasswordToken != model.Token)
            {
                TempData["Error"] = "Invalid or tampered reset link.";
                return View("InvalidResetPassword");
            }

            if (user.ResetPasswordTokenExpiry < DateTime.Now)
            {
                TempData["Warning"] = "Reset link has expired.";
                return View("InvalidResetPassword");
            }

            user.Password = model.NewPassword;

            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            await _userService.UpdateUserAsync(user);

            TempData["Success"] = "Password changed successfully.";

            return View("ResetPasswordSuccess");
        }
    }
}