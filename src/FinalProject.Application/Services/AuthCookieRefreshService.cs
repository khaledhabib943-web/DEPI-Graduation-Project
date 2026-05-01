using FinalProject.Domain.Entities;

namespace FinalProject.Application.Services
{
    /// <summary>
    /// Responsible solely for refreshing the Authentication Cookie
    /// after user data has been updated by any other component in the system.
    ///
    /// Design Decisions:
    /// - Stateless service: no session state or HTTP context stored as field.
    /// - SignInManager.RefreshSignInAsync re-issues the cookie with the latest
    ///   user claims without triggering a full sign-out/sign-in cycle, preserving
    ///   all existing session data and avoiding unnecessary redirects.
    /// - Returns a typed Result so callers can react without catching exceptions.
    ///
    /// NOTE: The concrete implementation lives in FinalProject.Infrastructure.Services.
    /// The Application layer (plain Microsoft.NET.Sdk class library) cannot reference
    /// SignInManager&lt;T&gt; — that type requires the Microsoft.AspNetCore.App framework
    /// reference, which is declared only in the Infrastructure project.
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
