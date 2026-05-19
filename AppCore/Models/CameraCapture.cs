namespace AppCore.Models;

public class CameraCapture
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public DateTime CaptureTime { get; set; } = DateTime.UtcNow;
}