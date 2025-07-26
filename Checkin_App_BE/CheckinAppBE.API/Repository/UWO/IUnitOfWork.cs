using Repository.Models;
using Repository.Repositories;

namespace Repository.UWO
{
    public interface IUnitOfWork
    {
        IGenericRepository<Badge> Badge { get; }
        IGenericRepository<Landmark> Landmark { get; }
        IGenericRepository<LandmarkVisit> LandmarkVisit { get; }
        IGenericRepository<LocalAuthentication> LocalAuthentication { get; }
        IGenericRepository<Mission> Mission { get; }
        IGenericRepository<Role> Role { get; }
        IGenericRepository<SocialAuthentication> SocialAuthentication { get; }
        IGenericRepository<StressLog> StressLog { get; }
        IGenericRepository<Treasure> Treasure { get; }
        IGenericRepository<User> User { get; }
        IGenericRepository<UserBadge> UserBadge { get; }
        IGenericRepository<UserMission> UserMission { get; }
        IGenericRepository<UserRole> UserRole { get; }
        IGenericRepository<UserTreasure> UserTreasure { get; }
        IGenericRepository<UserSession> UserSession { get; }
        Task<int> CommitAsync();
    }
}
