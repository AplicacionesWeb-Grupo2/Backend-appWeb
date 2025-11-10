namespace Back_End_tb4.Domain.Entities;

public class Plan
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;   // Free, Basic, Premium
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
}