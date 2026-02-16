using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentParliamentSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "coworking_statuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coworking_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "department_roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "event_tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "organization_request_statuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_request_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "department_members",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentRoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department_members", x => new { x.DepartmentId, x.UserId });
                    table.UniqueConstraint("AK_department_members_UserId_DepartmentId", x => new { x.UserId, x.DepartmentId });
                    table.ForeignKey(
                        name: "FK_department_members_department_roles_DepartmentRoleId",
                        column: x => x.DepartmentRoleId,
                        principalTable: "department_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_department_members_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_department_members_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    PosterUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_events_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users_roles",
                columns: table => new
                {
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_roles", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_users_roles_roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_roles_users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "coworking_bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpaceManagerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StartTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coworking_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_coworking_bookings_coworking_statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "coworking_statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_coworking_bookings_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_coworking_bookings_users_SpaceManagerId",
                        column: x => x.SpaceManagerId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "event_organizers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_organizers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_organizers_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_organizers_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_registrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisteredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_registrations_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_registrations_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "events_tags",
                columns: table => new
                {
                    EventsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events_tags", x => new { x.EventsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_events_tags_event_tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "event_tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_events_tags_events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organization_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_organization_requests_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_organization_requests_organization_request_statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "organization_request_statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_organization_requests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_coworking_bookings_EventId",
                table: "coworking_bookings",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_coworking_bookings_SpaceManagerId",
                table: "coworking_bookings",
                column: "SpaceManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_coworking_bookings_StatusId",
                table: "coworking_bookings",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_department_members_DepartmentRoleId",
                table: "department_members",
                column: "DepartmentRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_event_organizers_EventId_UserId",
                table: "event_organizers",
                columns: new[] { "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_organizers_UserId",
                table: "event_organizers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_event_registrations_EventId_UserId",
                table: "event_registrations",
                columns: new[] { "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_registrations_UserId",
                table: "event_registrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_events_CreatedByUserId",
                table: "events",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_events_tags_TagsId",
                table: "events_tags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_organization_requests_EventId_UserId",
                table: "organization_requests",
                columns: new[] { "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_organization_requests_StatusId",
                table: "organization_requests",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_organization_requests_UserId",
                table: "organization_requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_roles_UsersId",
                table: "users_roles",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coworking_bookings");

            migrationBuilder.DropTable(
                name: "department_members");

            migrationBuilder.DropTable(
                name: "event_organizers");

            migrationBuilder.DropTable(
                name: "event_registrations");

            migrationBuilder.DropTable(
                name: "events_tags");

            migrationBuilder.DropTable(
                name: "organization_requests");

            migrationBuilder.DropTable(
                name: "users_roles");

            migrationBuilder.DropTable(
                name: "coworking_statuses");

            migrationBuilder.DropTable(
                name: "department_roles");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "event_tags");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "organization_request_statuses");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
