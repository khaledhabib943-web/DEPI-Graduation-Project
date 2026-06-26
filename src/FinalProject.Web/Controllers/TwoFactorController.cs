using FinalProject.Domain.Entities;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Encodings.Web;

namespace FinalProject.Web.Controllers
{
    [Authorize]
    public class TwoFactorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public TwoFactorController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // ===== SETUP 2FA (English) =====
        [HttpGet]
        public async Task<IActionResult> Setup()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var model = await BuildSetupViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(Setup2faViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                // Rebuild QR data since model doesn't round-trip the URI
                var rebuilt = await BuildSetupViewModel(user);
                model.SharedKey = rebuilt.SharedKey;
                model.AuthenticatorUri = rebuilt.AuthenticatorUri;
                return View(model);
            }

            // Verify the code from the authenticator app
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                model.Code.Trim());

            if (!isValid)
            {
                var rebuilt = await BuildSetupViewModel(user);
                model.SharedKey = rebuilt.SharedKey;
                model.AuthenticatorUri = rebuilt.AuthenticatorUri;
                model.ErrorMessage = "Invalid verification code. Please scan the QR code again and enter a new code.";
                return View(model);
            }

            // Enable 2FA
            await _userManager.SetTwoFactorEnabledAsync(user, true);

            // Generate recovery codes
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            return RedirectToAction("RecoveryCodes", new { codes = string.Join(",", recoveryCodes!) });
        }

        // ===== RECOVERY CODES =====
        [HttpGet]
        public IActionResult RecoveryCodes(string codes)
        {
            if (string.IsNullOrEmpty(codes))
                return RedirectToAction("Index", "Profile");

            var model = new RecoveryCodesViewModel
            {
                RecoveryCodes = codes.Split(',', StringSplitOptions.RemoveEmptyEntries)
            };
            return View(model);
        }

        // ===== DISABLE 2FA =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Disable 2FA
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            // Reset the authenticator key so old codes won't work
            await _userManager.ResetAuthenticatorKeyAsync(user);

            return RedirectToAction("Index", "Profile");
        }

        // ===== SETUP 2FA (Arabic) =====
        [HttpGet]
        public async Task<IActionResult> SetupAr()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("LoginAr", "Account");

            var model = await BuildSetupViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetupAr(Setup2faViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("LoginAr", "Account");

            if (!ModelState.IsValid)
            {
                var rebuilt = await BuildSetupViewModel(user);
                model.SharedKey = rebuilt.SharedKey;
                model.AuthenticatorUri = rebuilt.AuthenticatorUri;
                return View(model);
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                model.Code.Trim());

            if (!isValid)
            {
                var rebuilt = await BuildSetupViewModel(user);
                model.SharedKey = rebuilt.SharedKey;
                model.AuthenticatorUri = rebuilt.AuthenticatorUri;
                model.ErrorMessage = "رمز التحقق غير صالح. يرجى مسح رمز QR مرة أخرى وإدخال رمز جديد.";
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            return RedirectToAction("RecoveryCodesAr", new { codes = string.Join(",", recoveryCodes!) });
        }

        [HttpGet]
        public IActionResult RecoveryCodesAr(string codes)
        {
            if (string.IsNullOrEmpty(codes))
                return RedirectToAction("IndexAr", "Profile");

            var model = new RecoveryCodesViewModel
            {
                RecoveryCodes = codes.Split(',', StringSplitOptions.RemoveEmptyEntries)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableAr()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("LoginAr", "Account");

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);

            return RedirectToAction("IndexAr", "Profile");
        }

        // ===== HELPERS =====
        private async Task<Setup2faViewModel> BuildSetupViewModel(User user)
        {
            // Get or create the authenticator key
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            // Format the key for display (groups of 4)
            var formattedKey = FormatKey(unformattedKey!);

            // Build the otpauth:// URI for QR code
            var email = await _userManager.GetEmailAsync(user);
            var uri = string.Format(
                AuthenticatorUriFormat,
                UrlEncoder.Default.Encode("Salahly"),
                UrlEncoder.Default.Encode(email!),
                unformattedKey);

            return new Setup2faViewModel
            {
                SharedKey = formattedKey,
                AuthenticatorUri = uri
            };
        }

        private static string FormatKey(string unformattedKey)
        {
            var sb = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                sb.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                sb.Append(unformattedKey.AsSpan(currentPosition));
            }
            return sb.ToString().ToLowerInvariant();
        }
    }
}
