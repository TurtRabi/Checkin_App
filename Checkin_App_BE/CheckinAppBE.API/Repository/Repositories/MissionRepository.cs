using Repository.Models;

namespace Repository.Repositories
{
    public class MissionRepository : GenericRepository<Mission>, IMissionRepository
    {
        public MissionRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}