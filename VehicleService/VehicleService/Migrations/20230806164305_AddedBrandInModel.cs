using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.VehicleService.Migrations
{
    public partial class AddedBrandInModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "brand_id",
                table: "model",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_model_brand_id",
                table: "model",
                column: "brand_id");

            migrationBuilder.AddForeignKey(
                name: "fk_model_brand_brand_id",
                table: "model",
                column: "brand_id",
                principalTable: "brand",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_model_brand_brand_id",
                table: "model");

            migrationBuilder.DropIndex(
                name: "ix_model_brand_id",
                table: "model");

            migrationBuilder.DropColumn(
                name: "brand_id",
                table: "model");
        }
    }
}
