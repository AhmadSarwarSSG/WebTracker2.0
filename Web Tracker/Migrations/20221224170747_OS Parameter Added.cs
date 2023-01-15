using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalRCheck.Migrations
{
    /// <inheritdoc />
    public partial class OSParameterAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OS",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OS",
                table: "Users");
        }
    }
}
