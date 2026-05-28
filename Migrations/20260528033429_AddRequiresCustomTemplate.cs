using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRequiresCustomTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "requires_custom_template",
                table: "request_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "requires_custom_template",
                table: "request_types");
        }
    }
}
