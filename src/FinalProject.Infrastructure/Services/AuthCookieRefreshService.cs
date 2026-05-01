using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using FinalProject.Application.Services;
using FinalProject.Domain.Entities;

namespace FinalProject.Infrastructure.Services
{
    /// <inheritdoc />
    public sealed class AuthCookieRefreshService : IAuthCookieRefreshService
    {
        // ── Dependencies ────────────────────────────────────────────────────────────

        private readonly UserManager<User>  _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthCookieRefreshService> _logger;

        // ── Constructor ─────────────────────────────────────────────────────────────

        public AuthCookieRefreshService(
            UserManager<User>     userManager,
            SignInManager<User>   signInManager,
            ILogger<AuthCookieRefreshService> logger)
        {
            _userManager   = userManager   ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _logger        = logger        ?? throw new ArgumentNullException(nameof(logger));
        }

        // ── Public API ───────────────────────────────────────────────────────────────

        /// <inheritdoc />
        public async Task<CookieRefreshResult> RefreshAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning(
                    "[AuthCookieRefresh] RefreshAsync called with a null or empty userId.");
                return CookieRefreshResult.UserNotFound;
            }

            cancellationToken.ThrowIfCancellationRequested();

            User? user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                _logger.LogWarning(
                    "[AuthCookieRefresh] User with Id '{UserId}' was not found in the store.",
                    userId);
                return CookieRefreshResult.UserNotFound;
            }

            return await RefreshAsync(user, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CookieRefreshResult> RefreshAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            if (user is null)
            {
                _logger.LogWarning(
                    "[AuthCookieRefresh] RefreshAsync called with a null user entity.");
                return CookieRefreshResult.UserNotFound;
            }

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await _signInManager.RefreshSignInAsync(user);

                _logger.LogInformation(
                    "[AuthCookieRefresh] Cookie refreshed for user '{UserId}'.",
                    user.Id);

                return CookieRefreshResult.Success;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[AuthCookieRefresh] Unexpected error refreshing cookie for user '{UserId}'.",
                    user.Id);

                return CookieRefreshResult.Failed;
            }
        }
    }
}
