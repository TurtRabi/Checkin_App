
using Repository.Models;

namespace Repository.Repositories
{
    public class RewardCardRepository : GenericRepository<RewardCard>, IRewardCardRepository
    {
        public RewardCardRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}
