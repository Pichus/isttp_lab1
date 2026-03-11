using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentParliamentSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDepartmentRoleEntityAndReferencesToIt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_department_members_department_roles_DepartmentRoleId",
                schema: "main_application",
                table: "department_members");

            migrationBuilder.DropTable(
                name: "department_roles",
                schema: "main_application");

            migrationBuilder.DropIndex(
                name: "IX_department_members_DepartmentRoleId",
                schema: "main_application",
                table: "department_members");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "department_roles",
                schema: "main_application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department_roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_department_members_DepartmentRoleId",
                schema: "main_application",
                table: "department_members",
                column: "DepartmentRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_department_members_department_roles_DepartmentRoleId",
                schema: "main_application",
                table: "department_members",
                column: "DepartmentRoleId",
                principalSchema: "main_application",
                principalTable: "department_roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
