namespace Back_End_tb4.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PsychologistId { get; set; }
    public DateTime DateTime { get; set; }
    public string Status { get; set; } = "Scheduled";
}