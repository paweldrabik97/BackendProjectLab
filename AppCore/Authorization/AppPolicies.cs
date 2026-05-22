public enum AppPolicies
{
    AdminOnly,
    ActiveUser,
    SalesDepartment

}

public static class AppPoliciesExtensions
{
    public static string Name(this AppPolicies policy)
    {
        return policy.ToString();
    }
}