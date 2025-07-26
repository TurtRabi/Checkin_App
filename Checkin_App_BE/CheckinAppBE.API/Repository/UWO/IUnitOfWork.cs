using Repository.Models;
using Repository.Repositories;

namespace Repository.UWO
{
    public interface IUnitOfWork
    {
        IBadgeRepository BadgeRepository { get; }
        ILandmarkRepository LandmarkRepository { get; }
        ILandmarkVisitRepository LandmarkVisitRepository { get; }
        IGenericRepository<LocalAuthentication> LocalAuthenticationRepository { get; }
        IMissionRepository MissionRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        ISocialAuthenticationRepository SocialAuthenticationRepository { get; }
        IGenericRepository<StressLog> StressLogRepository { get; }
        ITreasureRepository TreasureRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IUserBadgeRepository UserBadgeRepository { get; }
        IUserMissionRepository UserMissionRepository { get; }
        IGenericRepository<UserRole> UserRoleRepository { get; }
        IUserTreasureRepository UserTreasureRepository { get; }
        IGenericRepository<UserSession> UserSessionRepository { get; }
        Task<int> CommitAsync();
    }
}
