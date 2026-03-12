using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentParliamentSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstNameAndLastNameColumnsToUserEntityAndRemoveNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "main_application",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "main_application",
                table: "users",
                type: "character varying(35)",
                maxLength: 35,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "main_application",
                table: "users",
                type: "character varying(35)",
                maxLength: 35,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "main_application",
                table: "users");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "main_application",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "main_application",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
