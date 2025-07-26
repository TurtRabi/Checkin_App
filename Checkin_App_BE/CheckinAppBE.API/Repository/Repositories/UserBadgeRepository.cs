using Repository.Models;

namespace Repository.Repositories
{
    public class UserBadgeRepository : GenericRepository<UserBadge>, IUserBadgeRepository
    {
        public UserBadgeRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}