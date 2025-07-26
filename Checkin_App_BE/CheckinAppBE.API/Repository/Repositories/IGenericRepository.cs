using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public interface IGenericRepository<T> 
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id, string includeProperties = "");
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Query();
        Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> expression, string includeProperties = "");
        Task<T?> GetSingleByConditionAsync(Expression<Func<T, bool>> expression, string includeProperties = "");
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string includeProperties = "");
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string includeProperties = "");
        Task AddRangeAsync(IEnumerable<T> entities);

    }
}