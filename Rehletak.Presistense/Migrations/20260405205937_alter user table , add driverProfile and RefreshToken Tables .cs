using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rehletak.Presistense.Migrations
{
    /// <inheritdoc />
    public partial class alterusertableadddriverProfileandRefreshTokenTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "apple_id",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "auth_provider",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "google_id",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_email_verified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_onboarding_complete",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_phone_verified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "rating",
                table: "Users",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "role",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "driver_profiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    license_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    car_model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    car_plate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_verified = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_driver_profiles_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    token_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_driver_profiles_userId",
                table: "driver_profiles",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_userId",
                table: "refresh_tokens",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "driver_profiles");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "apple_id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "auth_provider",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "full_name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "google_id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "is_email_verified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "is_onboarding_complete",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "is_phone_verified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "rating",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "role",
                table: "Users");
        }
    }
}
