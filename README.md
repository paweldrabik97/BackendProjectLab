Here's the improved `README.md` file, incorporating the new content while maintaining the existing structure and information:

# BackendProjectLab

## Autor

- Piotr Czechowski - [GitHub Profile](https://github.com/Piotr-Czechowski-It)
- Pawe³ Drabik - [GitHub Profile](https://github.com/paweldrabik97)

## Zrealizowane funkcje

- **Autentykacja JWT (access token + refresh token)**
  - Generowanie access tokenów z claims oraz mechanizm odœwie¿ania tokenów.
- **Zarz¹dzanie u¿ytkownikami i rolami (ASP.NET Core Identity)**
  - Seedowanie ról i przyk³adowych u¿ytkowników przy starcie (klasa `IdentityDbSeeder`).
  - Przypisywanie ról do u¿ytkowników.
- **API uwierzytelniaj¹ce**
  - Endpointy: `POST /api/auth/login`, `POST /api/auth/refresh`, `POST /api/auth/revoke`, `GET /api/auth/me`.
  - `AuthController` zwraca `AuthResponseDto` oraz dane zalogowanego u¿ytkownika odczytane z claims.
- **Obs³uga refresh tokenów w bazie danych**
  - Tworzenie, uniewa¿nianie i przechowywanie refresh tokenów (encja `RefreshToken`).
- **Logowanie i obs³uga b³êdów**
  - Logowanie operacji seedowania oraz b³êdów Identity.

## Link do repozytorium

[BackendProjectLab Repository](https://github.com/paweldrabik97/BackendProjectLab)

## Uruchomienie projektu

### Wymagania:

- .NET 9 SDK
- Visual Studio 2022 (opcjonalnie) lub `dotnet` CLI
- SQL Server lub inna baza zgodna z konfiguracj¹ projektu

### Szybki start (CLI):

1. Sklonuj repozytorium:

    ```bash
    git clone https://github.com/paweldrabik97/BackendProjectLab.git
    cd BackendProjectLab
    ```

2. Przywróæ pakiety:

    ```bash
    dotnet restore
    ```

3. Skonfiguruj po³¹czenie i ustawienia JWT:

   - Zaktualizuj `appsettings.Development.json` lub ustaw User Secrets / zmienne œrodowiskowe dla klucza JWT i connection stringa.

   Przyk³ad fragmentu `appsettings.Development.json`:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=.;Database=BackendProjectLab;Trusted_Connection=True;"
      },
      "Jwt": {
        "Issuer": "YourIssuer",
        "Audience": "YourAudience",
        "Secret": "SuperLongSecretKeyHere",
        "ExpirationInMinutes": 60,
        "RefreshTokenDays": 30
      }
    }
    ```

4. Zastosuj migracje EF (jeœli projekt u¿ywa migracji):

    ```bash
    dotnet ef database update --project Infrastructure --startup-project WebApi
    ```

5. Uruchom aplikacjê:

    ```bash
    dotnet run --project WebApi
    ```

### Uruchomienie w Visual Studio 2022:

1. Otwórz rozwi¹zanie w Visual Studio 2022.
2. Ustaw projekt `WebApi` jako projekt startowy.
3. Skonfiguruj `appsettings.Development.json` lub User Secrets z connection string i `Jwt:Secret`.
4. Wykonaj migracje przez __Tools > NuGet Package Manager > Package Manager Console__ lub terminal.
5. Uruchom przez __Debug > Start Debugging__ lub __Debug > Start Without Debugging__.

### Uwagi bezpieczeñstwa:

- Seed danych zawiera przyk³adowe has³a jawne w kodzie — u¿ywaj tylko w œrodowisku lokalnym.
- W œrodowisku produkcyjnym `Jwt:Secret` musi byæ bezpiecznie przechowywany (np. Azure Key Vault, AWS Secrets Manager, lub zmienne œrodowiskowe).
- SprawdŸ poprawnoœæ konfiguracji DbContext i connection stringów przed zastosowaniem migracji.

