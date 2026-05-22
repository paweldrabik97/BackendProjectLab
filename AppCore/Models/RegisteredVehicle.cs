namespace AppCore.Models;

public class RegisteredVehicle : EntityBase
{
    public required string UserId { get; set; }
    public required string PlateNumber { get; set; }
    public required string Brand { get; set; }
    public DateTime RegisteredAt { get; set; }
}
