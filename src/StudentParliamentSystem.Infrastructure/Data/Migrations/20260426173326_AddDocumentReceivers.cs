using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentParliamentSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentReceivers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "document_receivers",
                schema: "main_application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Position = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_receivers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document_receivers",
                schema: "main_application");
        }
    }
}
