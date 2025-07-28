using Service.AuthenticationService;
using Service.EmailService;
using Service.LandmarkService;
using Service.RoleService;
using Service.UserService;
using Service.AdminService;
using Service.LandmarkVisitService;
using Service.BadgeService;
using Service.MissionService;
using Service.TreasureService;
using Service.UserTreasureService;
using Service.StressLogService;
using Service.NotificationService;

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
            services.AddScoped<ITreasureService, TreasureService>();
            services.AddScoped<IUserTreasureService, UserTreasureService>();
            services.AddScoped<IStressLogService, StressLogService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddHttpClient<IGoNotificationClientService, GoNotificationClientService>();

            return services;
        }
    }
}