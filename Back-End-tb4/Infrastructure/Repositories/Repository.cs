using System.Linq.Expressions;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Back_End_tb4.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly EiraMindDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(EiraMindDbContext context)
    {
        Context = context;
        DbSet = Context.Set<T>();
    }

    public async Task<IEnumerable<T>> ListAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<T?> FindByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        DbSet.Remove(entity);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }
}