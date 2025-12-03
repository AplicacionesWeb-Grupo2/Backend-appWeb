using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_End_tb4.Domain.Entities;

public class Appointment
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    public int PsychologistId { get; set; }
        
    public string PsychologistName { get; set; }
        
    [Required]
    public int PatientId { get; set; }
        
    public string PatientName { get; set; }
        
    [Required]
    public string Date { get; set; } // Formato: "2024-10-25" (igual que frontend)
        
    [Required]
    public string Time { get; set; } // Formato: "10:00" (igual que frontend)
        
    public string Notes { get; set; }
        
    public string Status { get; set; } = "confirmed"; // "confirmed", "pending", "cancelled"
        
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    // Relaciones
    [ForeignKey("PsychologistId")]
    public virtual Psychologist Psychologist { get; set; }
        
    [ForeignKey("PatientId")]
    public virtual User Patient { get; set; }
}