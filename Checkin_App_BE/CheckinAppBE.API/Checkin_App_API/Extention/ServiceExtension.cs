using Service.AuthenticationService;
using Service.EmailService;
using Service.UserService;
using Service.RoleService;
using Service.LandmarkService;
using Service.LandmarkVisitService;
using Service.BadgeService;
using Service.MissionService;

namespace API_UsePrevention.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILandmarkService, LandmarkService>();
            services.AddScoped<ILandmarkVisitService, LandmarkVisitService>();
            services.AddScoped<IBadgeService, BadgeService>();
            services.AddScoped<IMissionService, MissionService>();

            return services;
        }
    }
}