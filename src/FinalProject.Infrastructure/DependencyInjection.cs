using FinalProject.Application.Interfaces;
using FinalProject.Application.Services;
using FinalProject.Domain.Entities;
using FinalProject.Infrastructure.DbContext;
using FinalProject.Infrastructure.Repositories;
using FinalProject.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinalProject.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── 1. DbContext ──────────────────────────────────────────────
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // ── 2. ASP.NET Core Identity (with custom options) ────────────
            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 4;

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<Microsoft.AspNetCore.Identity.AuthenticatorTokenProvider<User>>(
                TokenOptions.DefaultAuthenticatorProvider);

            // ── 3. Repositories ──────────────────────────────────────────
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IWorkerRepository, WorkerRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();

            // ── 4. Unit of Work ──────────────────────────────────────────
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ── 5. Email Service ──────────────────────────────────────────
            services.AddTransient<IEmailSender, EmailSender>();

            // ── 6. Auth Cookie Refresh Service ────────────────────────────
            services.AddScoped<IAuthCookieRefreshService, AuthCookieRefreshService>();

            return services;
        }
    }
}