namespace PerfDemo.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
