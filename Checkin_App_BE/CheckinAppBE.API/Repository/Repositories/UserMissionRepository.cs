using Repository.Models;

namespace Repository.Repositories
{
    public class UserMissionRepository : GenericRepository<UserMission>, IUserMissionRepository
    {
        public UserMissionRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}