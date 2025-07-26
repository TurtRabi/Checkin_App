using Service.AuthenticationService;
using Service.EmailService;
using Service.UserService;
using Service.RoleService;
using Service.LandmarkService;
using Service.LandmarkVisitService;
using Service.BadgeService;
using Service.MissionService;
using Service.TreasureService;
using Service.UserTreasureService;
using Service.StressLogService;

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

            return services;
        }
    }
}