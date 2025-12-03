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
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Recording> Recordings => Set<Recording>();
    public DbSet<Plan> Plans => Set<Plan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
            
        // Configurar relaciones
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Psychologist)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PsychologistId)
            .OnDelete(DeleteBehavior.Restrict);
                
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
                
        modelBuilder.Entity<Recording>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);
                
        modelBuilder.Entity<Recording>()
            .HasOne(r => r.Psychologist)
            .WithMany()
            .HasForeignKey(r => r.PsychologistId);
    }
}