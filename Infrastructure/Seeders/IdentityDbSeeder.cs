using AppCore.Models;
using AppCore.Services;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public class IdentityDbSeeder : IDataSeeder
{
    public int Order => 1;

    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<IdentityDbSeeder> _logger;

    public IdentityDbSeeder(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ILogger<IdentityDbSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    // metoda interfejsowa
    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    // ── Role ──────────────────────────────────────────────
    // metoda dodająca role
    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
                new AppRole(UserRole.Administrator.ToString(),
                    "Pełny dostęp do systemu."),
	            // dodaj pozostałe role z opisem
	        };

        foreach (var role in roles)
        {
            // pomijamy tworznie roli, jeśli już istnieje
            if (await _roleManager.RoleExistsAsync(role.Name!))
                continue;

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                _logger.LogError(
                    "Błąd tworzenia roli {Role}: {Errors}",
                    role.Name, FormatErrors(result));
        }
    }

    // ── Użytkownicy ───────────────────────────────────────
    // metoda dodająca użytkowników
    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
                new SeedUser(
                    Id: "F5BADE14-6CC8-42A2-9A44-9842DA2D9280",
                    Email: "admin@app.pl",
                    FirstName: "Adam",
                    LastName: "Administrator",
                    Department: "IT",
                    Password: "Admin@123!",
                    Role: UserRole.Administrator
                ),
                new SeedUser(
                    Id: "93A7FFDD-057F-4021-9C68-FE06951FFA65",
                    Email: "jan.kowalski@app.pl",
                    FirstName: "Jan",
                    LastName: "Kowalski",
                    Department: "Sales",
                    Password: "Manager@123!",
                    Role: UserRole.Customer

                ),
                new SeedUser(
                    Id: "3D4769E2-1C75-43E1-A5BB-1F71C68E9F57",
                    Email: "anna.nowak@app.pl",
                    FirstName: "Anna",
                    LastName: "Nowak",
                    Department: "Sales",
                    Password: "Sales@123!",
                    Role: UserRole.Customer

                ),
                new SeedUser(
                    Id: "0E136AB2-1A6A-4A16-938D-84DFB0F64BBA",
                    Email: "piotr.wisniewski@app.pl",
                    FirstName: "Piotr",
                    LastName: "Wiśniewski",
                    Department: "Sales",
                    Password: "Piotr123!",
                    Role: UserRole.Customer

                ),
                new SeedUser(
                    Id: "76B253D6-C16C-470A-943C-92F314A090F2",
                    Email: "maria.wojcik@app.pl",
                    FirstName: "Maria",
                    LastName: "Wójcik",
                    Department: "Support",
                    Password: "Support@123!",
                    Role: UserRole.Customer

                ),
                new SeedUser(
                    Id: "E90A39C9-9CE2-400A-8A7B-8CF300D3B292",
                    Email: "tomasz.kaminski@app.pl",
                    FirstName: "Tomasz",
                    LastName: "Kamiński",
                    Department: "Management",
                    Password: "Readonly@123!",
                    Role: UserRole.Customer

                )
            };

        foreach (var seedUser in users)
            await CreateUserAsync(seedUser);
    }


    // Metoda pomocnicza tworząca 
    private async Task CreateUserAsync(SeedUser seedUser)
    {
        if (await _userManager.FindByEmailAsync(seedUser.Email) is not null)
        {
            _logger.LogInformation(
                "Użytkownik {Email} już istnieje — pomijam.", seedUser.Email);
            return;
        }

        // ✅ Poprawne tworzenie encji AppUser
        // Ustawiamy tylko właściwości które znamy —
        // UserManager uzupełni resztę automatycznie
        var user = new AppUser
        {
            // Stałe Id — powtarzalne między uruchomieniami
            // UserManager nie nadpisze Id jeśli jest już ustawione
            Id = seedUser.Id,
            UserName = seedUser.Email,
            NormalizedEmail = seedUser.Email.ToUpper(),
            Email = seedUser.Email,
            FullName = $"{seedUser.FirstName} {seedUser.LastName}",
            // Potwierdzenie emaila — seed omija weryfikację emaila
            EmailConfirmed = true,
            // Odblokowane konto od razu
            LockoutEnabled = true,
            LockoutEnd = null,
            AccessFailedCount = 0,
            // Wymagane dla 2FA — domyślnie wyłączone
            TwoFactorEnabled = false,
            PhoneNumberConfirmed = false,
            FirstName = seedUser.FirstName,
            LastName = seedUser.LastName,
            Department = seedUser.Department,
            Status = SystemUserStatus.Active,
        };

        user.Activate();

        // ✅ CreateAsync — hashuje hasło i uzupełnia wszystkie brakujące pola
        var createResult = await _userManager.CreateAsync(user, seedUser.Password);
        if (!createResult.Succeeded)
        {
            _logger.LogError(
                "Błąd tworzenia użytkownika {Email}: {Errors}",
                user.Email, createResult);
            return;
        }

        // ✅ AddToRoleAsync — po CreateAsync, gdy użytkownik istnieje już w bazie
        var roleResult = await _userManager
            .AddToRoleAsync(user, seedUser.Role.ToString());

        if (!roleResult.Succeeded)
        {
            _logger.LogError(
                "Błąd przypisania roli {Role} dla {Email}: {Errors}",
                seedUser.Role, seedUser.Email, FormatErrors(roleResult));
            return;
        }

        _logger.LogInformation(
            "Utworzono użytkownika {Email} z rolą {Role}.",
            seedUser.Email, seedUser.Role);
    }

    private static string FormatErrors(IdentityResult result) =>
        string.Join("; ", result.Errors.Select(e => e.Description));
}

// Rekord pomocniczy — dane seedowanego użytkownika, które możemy jawnie zainicjować
// klasy zarządzające z modułu Identity uzupełniają automacznie pozostałe właściwości
internal record SeedUser(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string Department,
    string Password,
    UserRole Role
);