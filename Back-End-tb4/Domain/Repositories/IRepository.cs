using System.Linq.Expressions;

namespace Back_End_tb4.Domain.Repositories;

public partial interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> ListAsync();
    Task<T?> FindByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<int> SaveChangesAsync();
}