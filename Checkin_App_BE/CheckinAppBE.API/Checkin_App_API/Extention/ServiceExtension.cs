using Service.AuthenticationService;
using Service.EmailService; // Add this line
using Service.UserService;
using Service.RoleService;

namespace API_UsePrevention.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IEmailService, EmailService>(); // Add this line

            return services;
        }
    }
}
