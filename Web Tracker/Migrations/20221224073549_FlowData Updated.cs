using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalRCheck.Migrations
{
    /// <inheritdoc />
    public partial class FlowDataUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "From",
                table: "FlowDatas");

            migrationBuilder.DropColumn(
                name: "To",
                table: "FlowDatas");

            migrationBuilder.AddColumn<string>(
                name: "Page",
                table: "FlowDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Page",
                table: "FlowDatas");

            migrationBuilder.AddColumn<int>(
                name: "From",
                table: "FlowDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "To",
                table: "FlowDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
