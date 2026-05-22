using AppCore.Models;

namespace AppCore.Dto;

public record RegisterVehicleDto(
    string PlateNumber,
    string Brand
);

public record TopUpDto(decimal Amount);

public record SessionPayDto(string PlateNumber);

public record SessionEntryDto(string PlateNumber, string GateName);

public record RegisteredVehicleDto(
    Guid Id,
    string PlateNumber,
    string Brand,
    DateTime RegisteredAt
);

public record DriverDiscountDto(
    Guid Id,
    DiscountType Type,
    bool IsActive,
    DateTime? ActivatedAt,
    DateTime? ExpiresAt
);

public record WalletDto(
    decimal Balance
);

public record WalletTransactionDto(
    Guid Id,
    decimal Amount,
    TransactionType Type,
    DateTime CreatedAt,
    Guid? SessionId
);
