using Back_End_tb4.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Back_End_tb4.Infrastructure.Persistence;

public class EiraMindDbContext : DbContext
{
    public EiraMindDbContext(DbContextOptions<EiraMindDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Psychologist> Psychologists => Set<Psychologist>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
}