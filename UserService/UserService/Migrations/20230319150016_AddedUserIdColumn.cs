using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.UserService.Migrations
{
    public partial class AddedUserIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "customers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_customers_user_id",
                table: "customers",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_customers_users_user_id",
                table: "customers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_customers_users_user_id",
                table: "customers");

            migrationBuilder.DropIndex(
                name: "ix_customers_user_id",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "customers");
        }
    }
}
