using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using FinalProject.Domain.Entities;

namespace FinalProject.Application.Services
{
    /// <summary>
    /// Responsible solely for refreshing the Authentication Cookie
    /// after user data has been updated by any other component in the system.
    ///
    /// Design Decisions:
    /// - Stateless service: no session state or HTTP context stored as field.
    /// - IHttpContextAccessor is injected to safely read the current request context.
    /// - SignInManager.RefreshSignInAsync re-issues the cookie with the latest
    ///   user claims without triggering a full sign-out/sign-in cycle, preserving
    ///   all existing session data and avoiding unnecessary redirects.
    /// - Returns a typed Result so callers can react without catching exceptions.
    ///
    /// Registration (add once in Program.cs / Startup.cs — DO NOT modify existing DI):
    ///   builder.Services.AddScoped&lt;IAuthCookieRefreshService, AuthCookieRefreshService&gt;();
    /// </summary>
    public interface IAuthCookieRefreshService
    {
        /// <summary>
        /// Refreshes the authentication cookie for the given user.
        /// Safe to call from any Service, Controller, or Razor Page
        /// without triggering Logout or Redirect.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cookie must be refreshed.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// <see cref="CookieRefreshResult.Success"/> when the cookie was refreshed successfully.<br/>
        /// <see cref="CookieRefreshResult.UserNotFound"/> when no user matches <paramref name="userId"/>.<br/>
        /// <see cref="CookieRefreshResult.Failed"/> on any unexpected error (details logged internally).
        /// </returns>
        Task<CookieRefreshResult> RefreshAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Overload that accepts a fully-loaded <see cref="User"/> instance
        /// to avoid a redundant database round-trip when the caller already has the entity.
        /// </summary>
        /// <param name="user">The already-loaded user entity.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Same result semantics as <see cref="RefreshAsync(string, CancellationToken)"/>.</returns>
        Task<CookieRefreshResult> RefreshAsync(User user, CancellationToken cancellationToken = default);
    }

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

            // Honour cancellation before hitting the database.
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
                // ── Core Refresh Logic ───────────────────────────────────────────────
                //
                // SignInManager.RefreshSignInAsync:
                //   1. Reads the latest user data (including updated Claims/Roles) from the store.
                //   2. Re-issues the authentication cookie with a new security stamp validation timestamp.
                //   3. Does NOT invalidate the current session or cause a redirect.
                //   4. Is the officially recommended ASP.NET Core Identity method for this purpose.
                //      Reference: https://docs.microsoft.com/aspnet/core/security/authentication/identity
                //
                // Alternative (used only when SignInManager is unavailable, e.g., background jobs):
                //   IUserClaimsPrincipalFactory<User> can build a new ClaimsPrincipal,
                //   followed by HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal).
                //   This is intentionally NOT used here because RefreshSignInAsync is safer: it
                //   validates the security stamp and respects all Identity configuration options.
                // ─────────────────────────────────────────────────────────────────────

                await _signInManager.RefreshSignInAsync(user);

                _logger.LogInformation(
                    "[AuthCookieRefresh] Authentication cookie successfully refreshed for user '{UserId}'.",
                    user.Id);

                return CookieRefreshResult.Success;
            }
            catch (OperationCanceledException)
            {
                // Re-throw cancellations — do not swallow them.
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[AuthCookieRefresh] Unexpected error while refreshing cookie for user '{UserId}'.",
                    user.Id);

                return CookieRefreshResult.Failed;
            }
        }
    }

    // ── Result Type ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Strongly-typed result returned by <see cref="IAuthCookieRefreshService"/>
    /// to allow callers to branch without catching exceptions.
    /// </summary>
    public enum CookieRefreshResult
    {
        /// <summary>Cookie was refreshed successfully.</summary>
        Success = 0,

        /// <summary>The specified user was not found in the Identity store.</summary>
        UserNotFound = 1,

        /// <summary>An unexpected error occurred. Details are written to the application logger.</summary>
        Failed = 2
    }
}
