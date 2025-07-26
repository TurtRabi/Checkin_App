using Repository.Models;

namespace Repository.Repositories
{
    public class TreasureRepository : GenericRepository<Treasure>, ITreasureRepository
    {
        public TreasureRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}