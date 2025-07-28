
using Repository.Models;

namespace Repository.Repositories
{
    public class UserRewardCardRepository : GenericRepository<UserRewardCard>, IUserRewardCardRepository
    {
        public UserRewardCardRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}
