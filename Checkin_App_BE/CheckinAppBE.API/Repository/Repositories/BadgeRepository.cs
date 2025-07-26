using Repository.Models;

namespace Repository.Repositories
{
    public class BadgeRepository : GenericRepository<Badge>, IBadgeRepository
    {
        public BadgeRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}