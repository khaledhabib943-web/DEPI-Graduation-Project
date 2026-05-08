using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(IUnitOfWork unitOfWork, UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // ================= LOGIN =================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectByRole();

            var model = new LoginViewModel { ReturnUrl = returnUrl };

            // Pre-fill from email confirmation or Google registration
            if (TempData["ConfirmedEmail"] is string confirmedEmail)
            {
                model.Username = confirmedEmail;
                model.SuccessMessage = "Email confirmed successfully! Please sign in to continue.";
            }
            else if (TempData["RegisteredEmail"] is string registeredEmail)
            {
                model.Username = registeredEmail;
                model.SuccessMessage = "Registration complete! Please sign in to continue.";
            }

            // Error messages from Google login redirect
            if (TempData["LoginError"] is string loginError)
                model.ErrorMessage = loginError;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await FindUserByCredentials(model.Username, model.Password);
            if (user == null)
            {
                model.ErrorMessage = "Invalid username or password.";
                return View(model);
            }

            if (!user.IsActive)
            {
                model.ErrorMessage = "Your account is deactivated.";
                return View(model);
            }

            // ── 2FA check ──
            if (user.TwoFactorEnabled)
            {
                TempData["2fa_UserId"] = user.Id;
                TempData["2fa_RememberMe"] = model.RememberMe;
                TempData["2fa_ReturnUrl"] = model.ReturnUrl;
                return RedirectToAction("Verify2fa");
            }

            await SignInUser(user, model.RememberMe);
            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("Index", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("Index", "WorkerDashboard"),
                _ => !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
                    ? Redirect(model.ReturnUrl)
                    : RedirectToAction("Index", "Dashboard")
            };
        }

        // ================= VERIFY 2FA (English) =================
        [HttpGet]
        public IActionResult Verify2fa()
        {
            if (TempData.Peek("2fa_UserId") == null)
                return RedirectToAction("Login");

            return View(new Verify2faViewModel
            {
                RememberMe = TempData.Peek("2fa_RememberMe") as bool? ?? false,
                ReturnUrl = TempData.Peek("2fa_ReturnUrl") as string
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify2fa(Verify2faViewModel model)
        {
            var userId = TempData.Peek("2fa_UserId");
            if (userId == null) return RedirectToAction("Login");
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user == null) return RedirectToAction("Login");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                model.Code.Trim());

            if (!isValid)
            {
                model.ErrorMessage = "Invalid verification code. Please try again.";
                return View(model);
            }

            TempData.Remove("2fa_UserId");
            TempData.Remove("2fa_RememberMe");
            TempData.Remove("2fa_ReturnUrl");

            await SignInUser(user, model.RememberMe);
            return RedirectByRole();
        }

        // ================= REGISTER =================
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
            var model = new RegisterViewModel();
            if (TempData["RegisterError"] is string error)
                model.ErrorMessage = error;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingEmail = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email);
            if (existingEmail.Any()) { model.ErrorMessage = "هذا البريد الإلكتروني مسجل مسبقاً."; return View(model); }

            var existingNid = await _unitOfWork.Customers.FindAsync(c => c.NationalId == model.NationalId);
            if (existingNid.Any()) { model.ErrorMessage = "الرقم القومي مسجل مسبقاً."; return View(model); }

            var customer = new Customer
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                UserName = model.Username.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                NationalId = model.NationalId.Trim(),
                Age = model.Age,
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Address = model.Address.Trim()
            };

            var result = await _userManager.CreateAsync(customer, model.Password);
            if (!result.Succeeded)
            {
                model.ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
                return View(model);
            }

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(customer);
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = customer.Id, token = token },
                protocol: Request.Scheme);

            var emailSubject = "Confirm your Salahly account";
            var emailBody = $@"
                <h2>Welcome to Salahly!</h2>
                <p>Thank you for registering. Please confirm your email address by clicking the link below:</p>
                <p><a href='{callbackUrl}'>Confirm Email</a></p>
                <p>If you didn't create an account with Salahly, please ignore this email.</p>";

            try { await _emailSender.SendEmailAsync(customer.Email, emailSubject, emailBody); }
            catch { /* Account created – email will need to be resent */ }
            return RedirectToAction("RegisterConfirmation");
        }

        // ================= EMAIL CONFIRMATION =================
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Redirect to login with email pre-filled instead of auto-sign-in
                TempData["ConfirmedEmail"] = user.Email;
                return RedirectToAction("Login");
            }

            return View("Error");
        }

        [HttpGet]
        public IActionResult RegisterConfirmation() => View();

        // ================= LOGIN AR =================
        [HttpGet]
        public IActionResult LoginAr(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectByRole(arabic: true);
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAr(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await FindUserByCredentials(model.Username, model.Password);
            if (user == null)
            {
                model.ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة.";
                return View(model);
            }

            if (!user.IsActive)
            {
                model.ErrorMessage = "تم إيقاف حسابك. يرجى التواصل مع الدعم الفني.";
                return View(model);
            }

            // ── 2FA check (Arabic) ──
            if (user.TwoFactorEnabled)
            {
                TempData["2fa_UserId"] = user.Id;
                TempData["2fa_RememberMe"] = model.RememberMe;
                TempData["2fa_ReturnUrl"] = model.ReturnUrl;
                TempData["2fa_Arabic"] = true;
                return RedirectToAction("Verify2faAr");
            }

            await SignInUser(user, model.RememberMe);
            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("IndexAr", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("IndexAr", "WorkerDashboard"),
                _ => RedirectToAction("IndexAr", "Dashboard")
            };
        }

        // ================= VERIFY 2FA (Arabic) =================
        [HttpGet]
        public IActionResult Verify2faAr()
        {
            if (TempData.Peek("2fa_UserId") == null)
                return RedirectToAction("LoginAr");

            return View(new Verify2faViewModel
            {
                RememberMe = TempData.Peek("2fa_RememberMe") as bool? ?? false,
                ReturnUrl = TempData.Peek("2fa_ReturnUrl") as string
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify2faAr(Verify2faViewModel model)
        {
            var userId = TempData.Peek("2fa_UserId");
            if (userId == null) return RedirectToAction("LoginAr");
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user == null) return RedirectToAction("LoginAr");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                model.Code.Trim());

            if (!isValid)
            {
                model.ErrorMessage = "رمز التحقق غير صالح. يرجى المحاولة مرة أخرى.";
                return View(model);
            }

            TempData.Remove("2fa_UserId");
            TempData.Remove("2fa_RememberMe");
            TempData.Remove("2fa_ReturnUrl");
            TempData.Remove("2fa_Arabic");

            await SignInUser(user, model.RememberMe);
            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("IndexAr", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("IndexAr", "WorkerDashboard"),
                _ => RedirectToAction("IndexAr", "Dashboard")
            };
        }

        // ================= REGISTER AR =================
        [HttpGet]
        public IActionResult RegisterAr()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("IndexAr", "Dashboard");
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAr(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingEmail = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email);
            if (existingEmail.Any()) { model.ErrorMessage = "هذا البريد الإلكتروني مسجل مسبقاً."; return View(model); }

            var existingNid = await _unitOfWork.Customers.FindAsync(c => c.NationalId == model.NationalId);
            if (existingNid.Any()) { model.ErrorMessage = "الرقم القومي مسجل مسبقاً."; return View(model); }

            var customer = new Customer
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                UserName = model.Username.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                NationalId = model.NationalId.Trim(),
                Age = model.Age,
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Address = model.Address.Trim()
            };

            var result = await _userManager.CreateAsync(customer, model.Password);
            if (!result.Succeeded)
            {
                model.ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
                return View(model);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(customer);
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = customer.Id, token = token },
                protocol: Request.Scheme);

            var emailSubject = "تأكيد حسابك في صالحly";
            var emailBody = $@"
                <h2>مرحباً بك في صالحly!</h2>
                <p>شكراً لتسجيلك. يرجى تأكيد عنوان بريدك الإلكتروني بالنقر على الرابط أدناه:</p>
                <p><a href='{callbackUrl}'>تأكيد البريد الإلكتروني</a></p>
                <p>إذا لم تقم بإنشاء حساب في صالحly، يرجى تجاهل هذا البريد الإلكتروني.</p>";

            try { await _emailSender.SendEmailAsync(customer.Email, emailSubject, emailBody); }
            catch { /* Account created – email will need to be resent */ }
            return RedirectToAction("RegisterConfirmationAr");
        }

        [HttpGet]
        public IActionResult RegisterConfirmationAr() => View();

        // ================= GOOGLE LOGIN (existing users only) =================
        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction("Login");

            var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            // Find existing user by email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // User is NOT registered — redirect to Register with error
                TempData["RegisterError"] = $"The email {email} is not registered. Please sign up first.";
                return RedirectToAction("Register");
            }

            // Link Google login if not already linked
            var logins = await _userManager.GetLoginsAsync(user);
            if (!logins.Any(l => l.LoginProvider == info.LoginProvider))
                await _userManager.AddLoginAsync(user, info);

            // ── 2FA check (same flow as manual login) ──
            if (user.TwoFactorEnabled)
            {
                TempData["2fa_UserId"] = user.Id;
                TempData["2fa_RememberMe"] = false;
                TempData["2fa_ReturnUrl"] = (string?)null;
                return RedirectToAction("Verify2fa");
            }

            // Sign in with proper role claims
            await SignInUser(user, isPersistent: false);

            // Redirect based on role
            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("Index", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("Index", "WorkerDashboard"),
                _ => RedirectToAction("Index", "Dashboard")
            };
        }

        // ================= GOOGLE SIGN-UP (new users) =================
        [HttpPost]
        public IActionResult ExternalRegister(string provider)
        {
            var redirectUrl = Url.Action("ExternalRegisterCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalRegisterCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction("Register");

            var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var fullName = info.Principal.FindFirst(ClaimTypes.Name)?.Value ?? "";
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Register");

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                TempData["LoginError"] = "This email is already registered. Please sign in instead.";
                return RedirectToAction("Login");
            }

            // Store Google info in TempData for the completion form
            TempData["GoogleEmail"] = email;
            TempData["GoogleFullName"] = fullName;

            return RedirectToAction("CompleteGoogleRegistration");
        }

        [HttpGet]
        public IActionResult CompleteGoogleRegistration()
        {
            var email = TempData.Peek("GoogleEmail") as string;
            var fullName = TempData.Peek("GoogleFullName") as string;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Register");

            return View(new CompleteGoogleRegistrationViewModel
            {
                Email = email,
                FullName = fullName ?? ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteGoogleRegistration(CompleteGoogleRegistrationViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Validate uniqueness
            var existingEmail = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email);
            if (existingEmail.Any()) { model.ErrorMessage = "This email is already registered."; return View(model); }

            var existingNid = await _unitOfWork.Customers.FindAsync(c => c.NationalId == model.NationalId);
            if (existingNid.Any()) { model.ErrorMessage = "This National ID is already registered."; return View(model); }

            var customer = new Customer
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                UserName = model.Username.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                NationalId = model.NationalId.Trim(),
                Age = model.Age,
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Address = model.Address.Trim(),
                EmailConfirmed = true // Google already verified the email
            };

            var result = await _userManager.CreateAsync(customer, model.Password);
            if (!result.Succeeded)
            {
                model.ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
                return View(model);
            }

            // Link Google login to this account (re-authenticate to get the info)
            // The Google link will happen on first Google sign-in via ExternalLoginCallback

            // Redirect to login with email pre-filled
            TempData["RegisteredEmail"] = model.Email;
            TempData.Remove("GoogleEmail");
            TempData.Remove("GoogleFullName");
            return RedirectToAction("Login");
        }

        // ================= LOGOUT & ACCESS DENIED =================
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ================= FORGOT / RESET PASSWORD =================
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { code },
                protocol: Request.Scheme);

            var emailSubject = "Reset Password";
            var emailBody = $@"
                <h2>Reset Your Password</h2>
                <p>Please reset your password by <a href='{callbackUrl}'>clicking here</a>.</p>
                <p>If you didn't request a password reset, you can safely ignore this email.</p>";

            try { await _emailSender.SendEmailAsync(model.Email, emailSubject, emailBody); }
            catch { /* Silently fail – don't reveal email delivery status */ }

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();

        [HttpGet]
        public IActionResult ResetPassword(string? code = null)
        {
            if (code == null) return BadRequest("A code must be supplied for password reset.");
            return View(new ResetPasswordViewModel { Code = code });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation() => View();

        // ================= FORGOT / RESET PASSWORD (Arabic) =================
        [HttpGet]
        public IActionResult ForgotPasswordAr() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPasswordAr(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return RedirectToAction("ForgotPasswordConfirmationAr");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                "ResetPasswordAr",
                "Account",
                new { code },
                protocol: Request.Scheme);

            var emailSubject = "إعادة تعيين كلمة المرور";
            var emailBody = $@"
                <div dir='rtl' style='text-align: right;'>
                    <h2>إعادة تعيين كلمة المرور الخاصة بك</h2>
                    <p>الرجاء إعادة تعيين كلمة المرور الخاصة بك عن طريق <a href='{callbackUrl}'>الضغط هنا</a>.</p>
                    <p>إذا لم تطلب إعادة تعيين كلمة المرور، يمكنك تجاهل هذا البريد الإلكتروني.</p>
                </div>";

            try { await _emailSender.SendEmailAsync(model.Email, emailSubject, emailBody); }
            catch { /* Silently fail – don't reveal email delivery status */ }

            return RedirectToAction("ForgotPasswordConfirmationAr");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmationAr() => View();

        [HttpGet]
        public IActionResult ResetPasswordAr(string? code = null)
        {
            if (code == null) return BadRequest("A code must be supplied for password reset.");
            return View(new ResetPasswordViewModel { Code = code });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordAr(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmationAr");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmationAr");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmationAr() => View();

        // ================= HELPERS =================
        private async Task<User?> FindUserByCredentials(string usernameOrEmail, string password)
        {
            var user = await _userManager.FindByNameAsync(usernameOrEmail)
                       ?? await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user == null) return null;
            return await _userManager.CheckPasswordAsync(user, password) ? user : null;
        }

        private async Task SignInUser(User user, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? user.Email ?? "Unknown"),
                new(ClaimTypes.Role, user.Role.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = isPersistent });
        }

        private IActionResult RedirectByRole(bool arabic = false)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            string action = arabic ? "IndexAr" : "Index";
            return role switch
            {
                "Admin" => RedirectToAction(action, "AdminDashboard"),
                "Worker" => RedirectToAction(action, "WorkerDashboard"),
                _ => RedirectToAction(action, "Dashboard")
            };
        }
    }
}