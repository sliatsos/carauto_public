using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.UserService.Migrations
{
    public partial class RemovedPasswordField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password",
                table: "users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
