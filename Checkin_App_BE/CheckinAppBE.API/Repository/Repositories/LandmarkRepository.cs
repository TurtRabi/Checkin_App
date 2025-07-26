using Repository.Models;

namespace Repository.Repositories
{
    public class LandmarkRepository : GenericRepository<Landmark>, ILandmarkRepository
    {
        public LandmarkRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}