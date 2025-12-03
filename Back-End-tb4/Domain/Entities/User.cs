using System.ComponentModel.DataAnnotations;

namespace Back_End_tb4.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    [EmailAddress]
    public string Email { get; set; }
        
    [Required]
    public string Password { get; set; }
        
    [Required]
    public string Nombre { get; set; }
        
    public string Tipo { get; set; } = "paciente"; // "paciente" o "psicologo"
        
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    // Relaciones
    public virtual ICollection<Appointment> Appointments { get; set; }
}