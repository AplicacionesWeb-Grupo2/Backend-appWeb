using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_End_tb4.Domain.Entities;

public class Recording
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    public int UserId { get; set; }
        
    [Required]
    public int PsychologistId { get; set; }
        
    public string PsychologistName { get; set; }
        
    public string SessionTopic { get; set; }
        
    public string Description { get; set; }
        
    public string Date { get; set; } // Formato: "2024-10-10"
        
    public int Duration { get; set; } // En minutos
        
    public string Thumbnail { get; set; }
        
    public string VideoUrl { get; set; }
        
    public bool Watched { get; set; }
        
    public string Notes { get; set; }
        
    // Relaciones
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
        
    [ForeignKey("PsychologistId")]
    public virtual Psychologist Psychologist { get; set; }
}