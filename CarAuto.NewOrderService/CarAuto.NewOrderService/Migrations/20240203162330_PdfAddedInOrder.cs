using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.OrderService.Migrations
{
    public partial class PdfAddedInOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pdf",
                table: "orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pdf",
                table: "orders");
        }
    }
}
