namespace AppCore.Models;

public class WalletTransaction : EntityBase
{
    public required string UserId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? SessionId { get; set; }
}

public enum TransactionType
{
    TopUp,
    SessionPayment
}
