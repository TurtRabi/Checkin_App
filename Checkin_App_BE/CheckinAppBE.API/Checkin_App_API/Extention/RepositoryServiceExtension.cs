using Repository.Repositories;
using Repository.UWO;

namespace API_UsePrevention.Extensions
{
    public static class RepositoryServiceExtension
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILandmarkRepository, LandmarkRepository>();
            services.AddScoped<ILandmarkVisitRepository, LandmarkVisitRepository>();
            services.AddScoped<IBadgeRepository, BadgeRepository>();
            services.AddScoped<IMissionRepository, MissionRepository>();
            services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();
            services.AddScoped<IUserMissionRepository, UserMissionRepository>();
            return services;
        }
    }
}