using AppCore.Models;
using AppCore.Enums;


namespace AppCore.Dto;

// --- CameraCapture ---

public record CameraCaptureDto(
    string LicensePlate,
    string Brand,
    string Color,
    string GateName,
    string? ImagePath = null
);

// --- Vehicle ---

public record VehicleDto(
    Guid Id,
    string LicensePlate,
    string Brand,
    string Color
);

// --- Parking Session ---

public record ParkingEntryResultDto(
    Guid SessionId,
    VehicleDto Vehicle,
    string GateName,
    DateTime EntryTime,
    string Message
);

public record ParkingExitResultDto(
    Guid SessionId,
    VehicleDto Vehicle,
    string GateName,
    DateTime EntryTime,
    DateTime ExitTime,
    TimeSpan Duration,
    TimeSpan FreeParkingDuration,
    decimal Fee,
    bool WasCharged,
    string Message
);

public record ActiveParkingSessionDto(
    Guid SessionId,
    VehicleDto Vehicle,
    string GateName,
    DateTime EntryTime,
    TimeSpan CurrentDuration
);

public record ParkingSessionHistoryDto(
    Guid SessionId,
    VehicleDto Vehicle,
    string GateName,
    DateTime EntryTime,
    DateTime? ExitTime,
    TimeSpan? Duration,
    decimal? Fee,
    bool IsActive
);

// --- Tariff ---

public record ParkingTariffDto(
    Guid Id,
    string Name,
    TimeSpan FreeParkingDuration,
    decimal HourlyRate,
    decimal DailyMaxRate,
    bool IsActive
);

public record CreateTariffDto(
    string Name,
    int FreeMinutes,
    decimal HourlyRate,
    decimal DailyMaxRate
);

// --- Gate ---

public record ParkingGateDto(
    Guid Id,
    string Name,
    string Type,
    string Location,
    bool IsOperational
)
{
    public ParkingGate ToEntity()
    {
        return new ParkingGate()
        {
            Id = Id,
            Name = Name,
            Type = Enum.Parse<GateType>(Type),
            Location = Location,
            IsOperational = IsOperational
        };
    }
};

public record CreateGateDto(
    string Name,
    string Type,
    string Location
);

// --- Stats ---

public record ParkingStatsDto(
    int ActiveVehicles,
    decimal TodayRevenue,
    int TodayEntries,
    int TodayExits
);