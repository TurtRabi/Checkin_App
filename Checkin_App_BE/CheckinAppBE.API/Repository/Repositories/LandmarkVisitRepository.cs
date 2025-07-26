using Repository.Models;

namespace Repository.Repositories
{
    public class LandmarkVisitRepository : GenericRepository<LandmarkVisit>, ILandmarkVisitRepository
    {
        public LandmarkVisitRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}