namespace AppCore.Models;

public class ParkingSession : EntityBase
{
    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }
    public string GateName { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public decimal? ParkingFee { get; set; }
    public bool IsActive { get; set; }
}