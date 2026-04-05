using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentParliamentSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixEventEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterUrl",
                schema: "main_application",
                table: "events");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                schema: "main_application",
                table: "events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTimeUtc",
                schema: "main_application",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTimeUtc",
                schema: "main_application",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_events_DepartmentId",
                schema: "main_application",
                table: "events",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_events_departments_DepartmentId",
                schema: "main_application",
                table: "events",
                column: "DepartmentId",
                principalSchema: "main_application",
                principalTable: "departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_departments_DepartmentId",
                schema: "main_application",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_DepartmentId",
                schema: "main_application",
                table: "events");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "main_application",
                table: "events");

            migrationBuilder.DropColumn(
                name: "EndTimeUtc",
                schema: "main_application",
                table: "events");

            migrationBuilder.DropColumn(
                name: "StartTimeUtc",
                schema: "main_application",
                table: "events");

            migrationBuilder.AddColumn<string>(
                name: "PosterUrl",
                schema: "main_application",
                table: "events",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
