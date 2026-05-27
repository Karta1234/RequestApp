using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RequestApp.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "request_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    is_terminal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_request_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "request_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_request_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    custom_template = table.Column<string>(type: "text", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_requests_request_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "request_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_requests_request_types_type_id",
                        column: x => x.type_id,
                        principalTable: "request_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "request_status_histories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    request_id = table.Column<int>(type: "integer", nullable: false),
                    from_status_id = table.Column<int>(type: "integer", nullable: true),
                    to_status_id = table.Column<int>(type: "integer", nullable: false),
                    changed_by = table.Column<int>(type: "integer", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_request_status_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_request_status_histories_request_statuses_from_status_id",
                        column: x => x.from_status_id,
                        principalTable: "request_statuses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_request_status_histories_request_statuses_to_status_id",
                        column: x => x.to_status_id,
                        principalTable: "request_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_request_status_histories_requests_request_id",
                        column: x => x.request_id,
                        principalTable: "requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "request_statuses",
                columns: new[] { "id", "is_terminal", "name" },
                values: new object[,]
                {
                    { 1, false, "New" },
                    { 2, false, "InReview" },
                    { 3, true, "Approved" },
                    { 4, true, "Rejected" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_request_status_histories_from_status_id",
                table: "request_status_histories",
                column: "from_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_status_histories_request_id",
                table: "request_status_histories",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_status_histories_to_status_id",
                table: "request_status_histories",
                column: "to_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_requests_employee_id_type_id",
                table: "requests",
                columns: new[] { "employee_id", "type_id" },
                unique: true,
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "ix_requests_status_id",
                table: "requests",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_requests_type_id",
                table: "requests",
                column: "type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "request_status_histories");

            migrationBuilder.DropTable(
                name: "requests");

            migrationBuilder.DropTable(
                name: "request_statuses");

            migrationBuilder.DropTable(
                name: "request_types");
        }
    }
}
