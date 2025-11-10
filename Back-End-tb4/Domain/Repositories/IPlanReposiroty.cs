using Back_End_tb4.Domain.Entities;

namespace Back_End_tb4.Domain.Repositories;

public interface IPlanRepository : IRepository<Plan>
{
    Task<Plan?> FindByNameAsync(string name);
}

public partial interface IRepository<T>
{
}