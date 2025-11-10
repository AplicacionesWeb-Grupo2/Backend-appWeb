using Back_End_tb4.Domain.Entities;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Back_End_tb4.Infrastructure.Repositories;

public class PlanRepository : Repository<Plan>, IPlanRepository
{
    public PlanRepository(EiraMindDbContext context) : base(context)
    {
    }

    public async Task<Plan?> FindByNameAsync(string name)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Name == name);
    }
}