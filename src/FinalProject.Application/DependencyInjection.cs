using FinalProject.Application.Interfaces;
using FinalProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinalProject.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register Application Services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IWorkerService, WorkerService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
