using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "620a583c-1d9d-472d-9047-a77237eef2bc");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "6dd24d84-65c2-470a-963e-e55c9312e71c", "63a1560c-bbdd-4d68-b051-475fff067fdb" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6dd24d84-65c2-470a-963e-e55c9312e71c");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "63a1560c-bbdd-4d68-b051-475fff067fdb");

            migrationBuilder.AddColumn<int>(
                name: "TotalSessions",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "WalletBalance",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "DriverDiscounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverDiscounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegisteredVehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    PlateNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Brand = table.Column<string>(type: "TEXT", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredVehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "32088f68-4472-4118-bc00-2b9afec11c6a", null, null, "Administrator", "ADMINISTRATOR" },
                    { "549a165d-2d53-47c0-ae47-9a25f23dad0b", null, null, "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DeactivatedAt", "Department", "Email", "EmailConfirmed", "FirstName", "FullName", "LastLoginAt", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Status", "TotalSessions", "TwoFactorEnabled", "UserName", "WalletBalance" },
                values: new object[] { "e24cf6df-8acf-4807-b8b6-d26925c406ee", 0, "b7560021-b068-446f-b7ee-a981152c80fe", new DateTime(2026, 5, 21, 11, 19, 12, 592, DateTimeKind.Utc).AddTicks(5659), null, "IT", "admin@parking.local", true, "Jan", "Jan Kowalski", null, "Kowalski", false, null, "ADMIN@PARKING.LOCAL", "ADMIN@PARKING.LOCAL", "AQAAAAIAAYagAAAAEAnnNzP8y9HiVzyIeX+vNCPguNyybY6svIy9bWF/6+/5M7EFh3KMYeK23qygEwm89A==", null, false, "49f07848-71a8-47c5-8e2b-84b816b01477", 0, 0, false, "admin@parking.local", 0m });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "32088f68-4472-4118-bc00-2b9afec11c6a", "e24cf6df-8acf-4807-b8b6-d26925c406ee" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverDiscounts");

            migrationBuilder.DropTable(
                name: "RegisteredVehicles");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "549a165d-2d53-47c0-ae47-9a25f23dad0b");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "32088f68-4472-4118-bc00-2b9afec11c6a", "e24cf6df-8acf-4807-b8b6-d26925c406ee" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "32088f68-4472-4118-bc00-2b9afec11c6a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e24cf6df-8acf-4807-b8b6-d26925c406ee");

            migrationBuilder.DropColumn(
                name: "TotalSessions",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WalletBalance",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "620a583c-1d9d-472d-9047-a77237eef2bc", null, null, "Customer", "CUSTOMER" },
                    { "6dd24d84-65c2-470a-963e-e55c9312e71c", null, null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DeactivatedAt", "Department", "Email", "EmailConfirmed", "FirstName", "FullName", "LastLoginAt", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Status", "TwoFactorEnabled", "UserName" },
                values: new object[] { "63a1560c-bbdd-4d68-b051-475fff067fdb", 0, "2300ea7a-1722-455f-92ac-b43f85b4acfb", new DateTime(2026, 5, 21, 7, 13, 21, 670, DateTimeKind.Utc).AddTicks(3641), null, "IT", "admin@parking.local", true, "Jan", "Jan Kowalski", null, "Kowalski", false, null, "ADMIN@PARKING.LOCAL", "ADMIN@PARKING.LOCAL", "AQAAAAIAAYagAAAAEMbKioxbd78zECI4ZA+1jZh1j1lmlXcsSBzMeKdOkUcc8vDRwyhmHfVKDDrJZrEUjg==", null, false, "0d5937a4-12f2-49f9-94cd-875240908e73", 0, false, "admin@parking.local" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "6dd24d84-65c2-470a-963e-e55c9312e71c", "63a1560c-bbdd-4d68-b051-475fff067fdb" });
        }
    }
}
