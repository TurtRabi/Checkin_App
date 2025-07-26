using Repository.Models;
using Repository.Repositories;

namespace Repository.UWO
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TravelCardsDBContext _context;
        public IGenericRepository<Badge> Badge { get; }

        public IGenericRepository<Landmark> Landmark { get; }

        public IGenericRepository<LandmarkVisit> LandmarkVisit { get; }

        public IGenericRepository<LocalAuthentication> LocalAuthentication { get; }

        public IGenericRepository<Mission> Mission { get; }

        public IGenericRepository<Role> Role { get; }

        public IGenericRepository<SocialAuthentication> SocialAuthentication { get; }

        public IGenericRepository<StressLog> StressLog { get; }

        public IGenericRepository<Treasure> Treasure { get; }

        public IGenericRepository<User> User { get; }

        public IGenericRepository<UserBadge> UserBadge { get; }

        public IGenericRepository<UserMission> UserMission { get; }

        public IGenericRepository<UserRole> UserRole { get; }

        public IGenericRepository<UserTreasure> UserTreasure { get; }

        public IGenericRepository<UserSession> UserSession { get; }
        public UnitOfWork(TravelCardsDBContext context)
        {
            _context = context;
            Badge = new GenericRepository<Badge>(_context);
            Landmark = new GenericRepository<Landmark>(_context);
            LandmarkVisit = new GenericRepository<LandmarkVisit>(_context);
            LocalAuthentication = new GenericRepository<LocalAuthentication>(_context);
            Mission = new GenericRepository<Mission>(_context);
            Role = new GenericRepository<Role>(_context);
            SocialAuthentication = new GenericRepository<SocialAuthentication>(_context);
            StressLog = new GenericRepository<StressLog>(_context);
            Treasure = new GenericRepository<Treasure>(_context);
            User = new GenericRepository<User>(_context);
            UserBadge = new GenericRepository<UserBadge>(_context);
            UserMission = new GenericRepository<UserMission>(_context);
            UserRole = new GenericRepository<UserRole>(_context);
            UserTreasure = new GenericRepository<UserTreasure>(_context);
            UserSession = new GenericRepository<UserSession>(_context);
        }



        public  async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
