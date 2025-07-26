using Repository.Models;

namespace Repository.Repositories
{
    public class UserTreasureRepository : GenericRepository<UserTreasure>, IUserTreasureRepository
    {
        public UserTreasureRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}