namespace AppCore.Services;

public interface IDataSeeder
{
    public int Order { get; }
    Task SeedAsync();
}
