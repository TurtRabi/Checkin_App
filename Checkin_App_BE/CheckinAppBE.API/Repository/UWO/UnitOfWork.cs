using Repository.Models;
using Repository.Repositories;

namespace Repository.UWO
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TravelCardsDBContext _context;
        public IBadgeRepository BadgeRepository { get; }
        public ILandmarkRepository LandmarkRepository { get; }
        public ILandmarkVisitRepository LandmarkVisitRepository { get; }
        public IGenericRepository<LocalAuthentication> LocalAuthenticationRepository { get; }
        public IMissionRepository MissionRepository { get; }
        public IGenericRepository<Role> RoleRepository { get; }
        public ISocialAuthenticationRepository SocialAuthenticationRepository { get; }
        public IGenericRepository<StressLog> StressLogRepository { get; }
        public ITreasureRepository TreasureRepository { get; }
        public IGenericRepository<User> UserRepository { get; }
        public IUserBadgeRepository UserBadgeRepository { get; }
        public IUserMissionRepository UserMissionRepository { get; }
        public IGenericRepository<UserRole> UserRoleRepository { get; }
        public IUserTreasureRepository UserTreasureRepository { get; }
        public IGenericRepository<UserSession> UserSessionRepository { get; }
        public IRewardCardRepository RewardCardRepository { get; }
        public IUserRewardCardRepository UserRewardCardRepository { get; }

        public UnitOfWork(TravelCardsDBContext context)
        {
            _context = context;
            BadgeRepository = new BadgeRepository(_context);
            LandmarkRepository = new LandmarkRepository(_context);
            LandmarkVisitRepository = new LandmarkVisitRepository(_context);
            LocalAuthenticationRepository = new GenericRepository<LocalAuthentication>(_context);
            MissionRepository = new MissionRepository(_context);
            RoleRepository = new GenericRepository<Role>(_context);
            SocialAuthenticationRepository = new SocialAuthenticationRepository(_context);
            StressLogRepository = new GenericRepository<StressLog>(_context);
            TreasureRepository = new TreasureRepository(_context);
            UserRepository = new GenericRepository<User>(_context);
            UserBadgeRepository = new UserBadgeRepository(_context);
            UserMissionRepository = new UserMissionRepository(_context);
            UserRoleRepository = new GenericRepository<UserRole>(_context);
            UserTreasureRepository = new UserTreasureRepository(_context);
            UserSessionRepository = new GenericRepository<UserSession>(_context);
            RewardCardRepository = new RewardCardRepository(_context);
            UserRewardCardRepository = new UserRewardCardRepository(_context);
        }



        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
