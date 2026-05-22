namespace AppCore.Models;

public static class DiscountConstants
{
    public static class RegistrationBonus
    {
        public const int ExtraFreeMinutes = 30;
        public const int ValidDays = 60;
    }

    public static class LoyaltyDiscount
    {
        public const decimal PriceReductionPercent = 20m;
        public const int ValidDays = 30;
        public const int RequiredSessions = 100;
    }
}
