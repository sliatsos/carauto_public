using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.UserService.Migrations
{
    public partial class AddedCustomerFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "image",
                table: "customers",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "country",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "email",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "image",
                table: "customers");
        }
    }
}
