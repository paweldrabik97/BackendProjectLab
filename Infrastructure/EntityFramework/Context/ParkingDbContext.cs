using AppCore.Models;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.EntityFramework.Context;

public class ParkingDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public DbSet<ParkingGate> Gates { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<RegisteredVehicle> RegisteredVehicles { get; set; }
    public DbSet<DriverDiscount> DriverDiscounts { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }

    public ParkingDbContext() { }
    public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options) { }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=parking.db");
        }
        
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Konfiguracja encji Identity
        builder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.Department).HasMaxLength(100);
        });

        // Konfiguracja Twoich encji parkingowych
        builder.Entity<ParkingGate>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(50);
            entity.Property(g => g.Location).HasMaxLength(100);
            entity.Property(g => g.Type).HasConversion<string>(); // Zapisywanie enuma jako string w bazie

            // Relacja: Jedna bramka ma wiele zdjęć z kamery
            entity.HasMany(g => g.CameraCaptures)
                  .WithOne()
                  .OnDelete(DeleteBehavior.Cascade); // Usunięcie bramki usuwa zdjęcia
        });

        // Seedowanie ról
        var adminRoleId = Guid.NewGuid().ToString();
        builder.Entity<AppRole>().HasData(
            new AppRole { Id = adminRoleId, Name = UserRole.Administrator.ToString(), NormalizedName = "ADMINISTRATOR" },
            new AppRole { Id = Guid.NewGuid().ToString(), Name = UserRole.Customer.ToString(), NormalizedName = "CUSTOMER" }
        );

        // Seedowanie użytkownika admina (przykład)
        var hasher = new PasswordHasher<AppUser>();
        var adminUser = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin@parking.local",
            NormalizedUserName = "ADMIN@PARKING.LOCAL",
            Email = "admin@parking.local",
            NormalizedEmail = "ADMIN@PARKING.LOCAL",
            EmailConfirmed = true,
            FirstName = "Jan",
            LastName = "Kowalski",
            FullName = "Jan Kowalski",
            Department = "IT",
            Status = SystemUserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");
        builder.Entity<AppUser>().HasData(adminUser);

        // Powiązanie usera z rolą
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminUser.Id }
        );
    }
}