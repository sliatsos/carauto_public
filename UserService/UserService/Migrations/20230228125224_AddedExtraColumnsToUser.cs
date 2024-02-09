using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.UserService.Migrations
{
    public partial class AddedExtraColumnsToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "county",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "mobile",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "post_code",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "city",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "county",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "mobile",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "post_code",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "type",
                table: "customers");
        }
    }
}
