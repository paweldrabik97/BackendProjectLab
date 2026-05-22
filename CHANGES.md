# Podsumowanie zmian — Lab 7

## AppCore — warstwa domeny (nowe pliki)

### Modele (`AppCore/Models/`)

**`RegisteredVehicle.cs`** — nowy
Encja pojazdu powiązanego z kontem użytkownika. Przechowuje `UserId`, `PlateNumber`, `Brand`, `RegisteredAt`. Oddzielna od `Vehicle` — `Vehicle` to anonimowy rekord wjazdu, `RegisteredVehicle` to pojazd należący do konta.

**`DriverDiscount.cs`** — nowy
Encja rabatu kierowcy + enum `DiscountType` (`RegistrationBonus`, `LoyaltyDiscount`). Pola `IsActive`, `ActivatedAt`, `ExpiresAt` modelują cykl życia rabatu: dostępny → aktywowany → wygasły.

**`WalletTransaction.cs`** — nowy
Encja transakcji portfela + enum `TransactionType` (`TopUp`, `SessionPayment`). `SessionId` jest nullable — wypełniony tylko przy płatności za sesję, pusty przy doładowaniu.

**`DiscountConstants.cs`** — nowy
Stałe domenowe dla rabatów w jednym miejscu.
- `RegistrationBonus`: 30 min extra, ważny 60 dni
- `LoyaltyDiscount`: 20% zniżki, ważny 30 dni, wymaga 100 sesji

**`TariffConstants.cs`** — nowy
Stałe taryfowe: 30 minut gratis, 5 PLN/h. Wydzielone osobno od `DiscountConstants` bo dotyczą cennika, nie rabatów.

---

### DTO (`AppCore/Dto/`)

**`AuthDto.cs`** — nowy
Zawiera `LoginDto`, `AuthResponseDto`, `RefreshTokenDto`, `UserDto`.
`AuthResponseDto` zostało rozszerzone o pole `User` — odpowiedź z logowania od razu zwraca dane użytkownika, żeby klient nie musiał robić dodatkowego zapytania do `/me`.
`UserDto` zawiera: `Id`, `Email`, `FirstName`, `LastName`, `FullName`, `Department`, `Position`, `Status`, `Roles`, `CreatedAt`, `LastLoginAt`.

**`DriverDto.cs`** — nowy
DTO dla funkcji kierowcy:
- `RegisterVehicleDto` — żądanie rejestracji pojazdu
- `RegisteredVehicleDto` — odpowiedź z danymi pojazdu
- `DriverDiscountDto` — dane rabatu
- `WalletDto` — saldo portfela
- `WalletTransactionDto` — pojedyncza transakcja
- `TopUpDto` — żądanie doładowania
- `SessionPayDto` — żądanie płatności za sesję

---

### Repozytoria (`AppCore/Repositories/`)

**`IRegisteredVehicleRepository.cs`** — nowy
Rozszerza `IGenericRepositoryAsync<RegisteredVehicle>` o:
- `GetByUserIdAsync` — wszystkie pojazdy użytkownika
- `GetByUserIdAndPlateAsync` — sprawdzanie duplikatów przy rejestracji

**`IDriverDiscountRepository.cs`** — nowy
Rozszerza bazowe repo o:
- `GetByUserIdAsync` — wszystkie rabaty użytkownika
- `GetActiveByUserIdAndTypeAsync` — filtruje po `IsActive = true` i `ExpiresAt > now`

**`IWalletRepository.cs`** — nowy
Rozszerza bazowe repo o `GetByUserIdAsync` z sortowaniem malejącym po dacie.

---

### Serwisy (`AppCore/Services/`)

**`IAuthService.cs`** — nowy
Trzy metody: `LoginAsync`, `RefreshTokenAsync`, `RevokeTokenAsync`.

**`IDataSeeder.cs`** — nowy
Interfejs seederów z właściwością `Order` (kolejność wykonania) i metodą `SeedAsync`. `Order` gwarantuje że role są tworzone przed użytkownikami.

**`IParkingSessionService.cs`** — nowy
- `EntryAsync(plateNumber, gateName)` — rejestracja wjazdu
- `ExitAsync(plateNumber, gateName)` — rejestracja wyjazdu
- `GetActiveSessionAsync(plateNumber)` — stan aktywnej sesji
- `PayAsync(plateNumber, userId?)` — płatność; opcjonalne `userId` aktywuje rabaty

**`IDriverService.cs`** — nowy
- `RegisterVehicleAsync(userId, dto)` — rejestracja pojazdu + przyznanie bonusu
- `GetVehiclesAsync(userId)` — lista pojazdów użytkownika
- `GetVehicleHistoryAsync(userId, vehicleId)` — historia sesji pojazdu

**`IDiscountService.cs`** — nowy
- `GetDiscountsAsync(userId)` — lista rabatów użytkownika
- `ActivateAsync(userId, type)` — aktywacja rabatu z walidacją eligibility
- `GrantRegistrationBonusAsync(userId)` — przyznanie bonusu (stan "dostępny")
- `ApplyDiscountsAsync(userId, fee, freeMinutes)` — przeliczenie opłaty z rabatami

**`IWalletService.cs`** — nowy
- `GetWalletAsync(userId)` — saldo portfela
- `TopUpAsync(userId, amount)` — doładowanie
- `PayFromWalletAsync(userId, sessionId, amount)` — płatność z konta
- `GetTransactionsAsync(userId)` — historia transakcji

---

## Infrastructure — warstwa infrastruktury

### Encje i kontekst

**`AppUser.cs`** — zmodyfikowany
Dodano `WalletBalance` i `TotalSessions`. `TotalSessions` jest używany do sprawdzania eligibility rabatu lojalnościowego (wymagane >= 100).

**`ParkingDbContext.cs`** — zmodyfikowany
Dodano trzy nowe `DbSet`:
```csharp
public DbSet<RegisteredVehicle> RegisteredVehicles { get; set; }
public DbSet<DriverDiscount> DriverDiscounts { get; set; }
public DbSet<WalletTransaction> WalletTransactions { get; set; }
```

---

### Repozytoria EF (`Infrastructure/EntityFramework/Repositories/`)

**`EfRegisteredVehicleRepository.cs`** — nowy
Implementacja `IRegisteredVehicleRepository` z LINQ i EF Core.

**`EfDriverDiscountRepository.cs`** — nowy
`GetActiveByUserIdAndTypeAsync` sprawdza trzy warunki jednocześnie: `UserId`, `Type`, `IsActive = true` i `ExpiresAt > DateTime.UtcNow`.

**`EfWalletRepository.cs`** — nowy
`GetByUserIdAsync` z `OrderByDescending(t => t.CreatedAt)`.

---

### Serwisy (`Infrastructure/Services/`)

**`ParkingSessionService.cs`** — nowy
Logika wjazdu/wyjazdu:
- Przy wjeździe tworzy anonimowy `Vehicle` jeśli nie istnieje w bazie
- Kalkulacja opłaty: `ceil(billableHours) * 5 PLN` gdzie `billableTime = duration - 30 min`
- `CloseSessionAsync` to wspólna prywatna metoda dla `ExitAsync` i `PayAsync` — różnią się tylko przekazaniem `gateName` i `userId`
- Przy zamknięciu sesji inkrementuje `AppUser.TotalSessions`

**`DiscountService.cs`** — nowy
- `ActivateAsync` sprawdza: czy LoyaltyDiscount wymaga 100 sesji, czy nie ma aktywnego rabatu tego typu, czy istnieje dostępny (nieaktywowany) rabat
- `ApplyDiscountsAsync` dodaje darmowe minuty z `RegistrationBonus` i redukuje cenę o % z `LoyaltyDiscount`
- `GrantRegistrationBonusAsync` tworzy rabat w stanie `IsActive = false` — kierowca musi go aktywować

**`WalletService.cs`** — nowy
- `TopUpAsync` i `PayFromWalletAsync` aktualizują `AppUser.WalletBalance` przez `UserManager`
- Saldo trzymane w tej samej tabeli co dane użytkownika — bez osobnej encji Wallet

**`DriverService.cs`** — nowy
- `RegisterVehicleAsync` — po zapisaniu liczy pojazdy użytkownika; jeśli to pierwszy, wywołuje `GrantRegistrationBonusAsync`
- `GetVehicleHistoryAsync` — weryfikuje własność pojazdu (`vehicle.UserId != userId`) przed zwróceniem historii

---

### Bezpieczeństwo (`Infrastructure/Security/`)

**`AuthService.cs`** — nowy
Implementacja JWT: login, refresh, revoke.
`GenerateAccessToken` koduje w tokenie: `NameIdentifier`, `Email`, `GivenName`, `Surname`, `department`, role — te same pola odczytuje endpoint `/me`.

**`JwtSettings.cs`** — nowy
Odczyt konfiguracji JWT z `appsettings.json`. Rzuca `InvalidOperationException` przy starcie jeśli brakuje klucza — fail fast zamiast cichego błędu runtime.

---

### Seedery (`Infrastructure/Seeders/`)

**`IdentityDbSeeder.cs`** — nowy
Seeduje role i 6 użytkowników testowych. Idempotentny — pomija istniejących użytkowników przez `FindByEmailAsync`. Użytkownicy:

| Email | Hasło | Rola |
|---|---|---|
| admin@app.pl | Admin@123! | Administrator |
| jan.kowalski@app.pl | Manager@123! | Customer |
| anna.nowak@app.pl | Sales@123! | Customer |
| piotr.wisniewski@app.pl | Piotr123! | Customer |
| maria.wojcik@app.pl | Support@123! | Customer |
| tomasz.kaminski@app.pl | Readonly@123! | Customer |

---

### Migracje (`Infrastructure/Migrations/`)

**`InitialCreate`** — nowa
Tworzy wszystkie tabele Identity + `Gates`, `RefreshTokens`.

**`AddDriverFeatures`** — nowa
Dodaje tabele: `RegisteredVehicles`, `DriverDiscounts`, `WalletTransactions`.
Dodaje kolumny `WalletBalance` i `TotalSessions` do `AspNetUsers`.

---

### `ParkingInfrastructureModule.cs` — zmodyfikowany
Zarejestrowano nowe repozytoria i serwisy:
```csharp
services.AddScoped<IRegisteredVehicleRepository, EfRegisteredVehicleRepository>();
services.AddScoped<IDriverDiscountRepository, EfDriverDiscountRepository>();
services.AddScoped<IWalletRepository, EfWalletRepository>();
services.AddScoped<IDiscountService, DiscountService>();
services.AddScoped<IWalletService, WalletService>();
services.AddScoped<IDriverService, DriverService>();
services.AddScoped<IParkingSessionService, ParkingSessionService>();
services.AddScoped<IAuthService, AuthService>();
```
Dodano metodę `AddJwt` z konfiguracją authentication i polityk autoryzacji.

---

## WebApi — warstwa prezentacji

### Kontrolery (`WebApi/Controller/`)

**`AuthController.cs`** — nowy

| Metoda | Endpoint | Auth | Opis |
|---|---|---|---|
| POST | `/api/auth/login` | Anonymous | Logowanie, zwraca JWT + dane użytkownika |
| POST | `/api/auth/refresh` | Anonymous | Odświeżenie access tokenu |
| POST | `/api/auth/revoke` | Bearer | Unieważnienie refresh tokenu |
| GET | `/api/auth/me` | Bearer | Dane zalogowanego użytkownika z claims |

**`SessionsController.cs`** — nowy

| Metoda | Endpoint | Auth | Opis |
|---|---|---|---|
| GET | `/api/sessions/{plate}/status` | Anonymous | Stan aktywnej sesji |
| POST | `/api/sessions/{plate}/pay` | Anonymous | Symulowana płatność, zamknięcie sesji |

**`DriverController.cs`** — nowy

| Metoda | Endpoint | Opis |
|---|---|---|
| POST | `/api/driver/vehicles` | Rejestracja pojazdu |
| GET | `/api/driver/vehicles` | Lista pojazdów użytkownika |
| GET | `/api/driver/vehicles/{id}/history` | Historia sesji pojazdu |
| GET | `/api/driver/sessions/{plate}/status` | Stan sesji (zalogowany) |
| POST | `/api/driver/sessions/pay` | Płatność z portfela (z rabatami) |
| GET | `/api/driver/discounts` | Lista rabatów |
| POST | `/api/driver/discounts/{type}/activate` | Aktywacja rabatu |
| GET | `/api/driver/wallet` | Saldo portfela |
| POST | `/api/driver/wallet/topup` | Doładowanie portfela |
| GET | `/api/driver/wallet/transactions` | Historia transakcji |

---

### `Program.cs` — zmodyfikowany
1. **Usunięto duplikaty** rejestracji `IParkingGateRepository` i `IParkingUnitOfWork` jako Singleton — konflikt Singleton vs Scoped powodował crash przy starcie (`Cannot consume scoped service from singleton`)
2. **Dodano automatyczne migracje** — `db.Database.Migrate()` przy każdym starcie aplikacji (bezpieczne — pomija już zastosowane)
3. **Dodano wywołanie seedera** — `IDataSeeder.SeedAsync()` przy starcie

### `appsettings.json` — zmodyfikowany
Dodano sekcję `Jwt`:
```json
"Jwt": {
  "Issuer": "ParkingApi",
  "Audience": "ParkingClient",
  "SecretKey": "SuperTajnyKluczDoJWT_MinimumDwadziesciaCztery!",
  "ExpiryInMinutes": 60,
  "RefreshTokenDays": 7
}
```
Bez tej sekcji `JwtSettings` rzuca `InvalidOperationException` przy pierwszym żądaniu wymagającym autoryzacji.

### `WebApi.http` — zmodyfikowany
Przepisany na pełny zestaw 13 testów z komentarzami. Zmienne `@token`, `@gateId`, `@captureId`, `@refreshToken` do kopiowania między żądaniami.

---

## Architektura przepływu danych

```
Anonimowy kierowca:
  GET /api/sessions/{plate}/status  →  ParkingSessionService.GetActiveSessionAsync
  POST /api/sessions/{plate}/pay    →  ParkingSessionService.PayAsync(plate, userId: null)

Zalogowany kierowca:
  POST /api/driver/vehicles          →  DriverService.RegisterVehicleAsync
                                         └→ DiscountService.GrantRegistrationBonusAsync (pierwszy pojazd)
  POST /api/driver/sessions/pay      →  ParkingSessionService.PayAsync(plate, userId)
                                         └→ DiscountService.ApplyDiscountsAsync
                                         └→ WalletService.PayFromWalletAsync
  POST /api/driver/discounts/{type}/activate  →  DiscountService.ActivateAsync
                                                   └→ sprawdza TotalSessions (LoyaltyDiscount)
```

## Logika biznesowa — kalkulacja opłaty

```
duration = exitTime - entryTime
billableTime = duration - 30 min (TariffConstants.FreeMinutes)

if billableTime <= 0:
    fee = 0
else:
    fee = ceil(billableTime.TotalHours) * 5 PLN

// Rabaty (tylko dla zalogowanych):
if RegistrationBonus aktywny:
    fee -= 30 min * (5 PLN / 60 min)   // dodatkowe darmowe minuty

if LoyaltyDiscount aktywny:
    fee *= 0.80                          // 20% zniżki
```
