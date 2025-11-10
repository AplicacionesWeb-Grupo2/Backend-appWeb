namespace Back_End_tb4.Domain.Entities;

public class Psychologist
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Specialty { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Rating { get; set; }
}