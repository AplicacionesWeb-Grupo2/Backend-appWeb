using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_End_tb4.Domain.Entities;

public class Psychologist
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
        
    public string Tipo { get; set; } = "psicologo";
        
    public string Especialidad { get; set; }
        
    [Range(1, 5)]
    public decimal Calificacion { get; set; } = 5;
        
    public string Imagen { get; set; }
        
    [Column(TypeName = "text")]
    public string Descripcion { get; set; }
        
    [Column(TypeName = "text")]
    public string Biografia { get; set; }
        
    public string Educacion { get; set; }
        
    // Para listas, las guardamos como texto separado por comas
    public string Certificaciones { get; set; } // "Cert1, Cert2, Cert3"
    public string Idiomas { get; set; } // "Español, Inglés"
    public string Metodologias { get; set; } // "TCC, Mindfulness, Terapia de Exposición"
        
    public int AnosExperiencia { get; set; }
        
    public string AtendeEdades { get; set; }
        
    // Horarios como JSON string (lo parsearemos en el frontend)
    [Column(TypeName = "text")]
    public string Horarios { get; set; }
        
    // Relaciones
    public virtual ICollection<Appointment> Appointments { get; set; }
}