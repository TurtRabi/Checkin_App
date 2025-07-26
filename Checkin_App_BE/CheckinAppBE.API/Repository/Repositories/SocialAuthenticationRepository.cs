using Repository.Models;

namespace Repository.Repositories
{
    public class SocialAuthenticationRepository : GenericRepository<SocialAuthentication>, ISocialAuthenticationRepository
    {
        public SocialAuthenticationRepository(TravelCardsDBContext context) : base(context)
        {
        }
    }
}
