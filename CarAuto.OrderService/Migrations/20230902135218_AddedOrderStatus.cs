using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.OrderService.Migrations
{
    public partial class AddedOrderStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "order_number",
                table: "orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order_number",
                table: "orders");
        }
    }
}
