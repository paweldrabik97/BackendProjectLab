namespace AppCore.Models;

public class DriverDiscount : EntityBase
{
    public required string UserId { get; set; }
    public DiscountType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public enum DiscountType
{
    RegistrationBonus,
    LoyaltyDiscount
}
