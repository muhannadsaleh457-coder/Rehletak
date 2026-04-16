using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rehletak.Presistense.Migrations
{
    /// <inheritdoc />
    public partial class editTorefreshtokentable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_Users_userId",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "token_hash",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "refresh_tokens",
                newName: "token");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "refresh_tokens",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_Users_userId",
                table: "refresh_tokens",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_Users_userId",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "refresh_tokens",
                newName: "user_id");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "refresh_tokens",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "token_hash",
                table: "refresh_tokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_Users_userId",
                table: "refresh_tokens",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
