namespace AppCore.Models;

public class ParkingTariff : EntityBase
{
    public string Name { get; set; }
    public TimeSpan FreeParkingDuration { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal DailyMaxRate { get; set; }
    bool IsActive { get; set; }
}