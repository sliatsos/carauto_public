using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.UserService.Migrations
{
    public partial class AddedUserIdColumnToSalesperson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "salespersons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_salespersons_user_id",
                table: "salespersons",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_salespersons_users_user_id",
                table: "salespersons",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_salespersons_users_user_id",
                table: "salespersons");

            migrationBuilder.DropIndex(
                name: "ix_salespersons_user_id",
                table: "salespersons");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "salespersons");
        }
    }
}
