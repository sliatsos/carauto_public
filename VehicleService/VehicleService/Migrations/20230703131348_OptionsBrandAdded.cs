using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.VehicleService.Migrations
{
    public partial class OptionsBrandAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "brand_id",
                table: "vehicles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "mileage_value",
                table: "vehicles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "model_id",
                table: "vehicles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "model_id",
                table: "option",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "model",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    model_year = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_model", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_brand_id",
                table: "vehicles",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_model_id",
                table: "vehicles",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "ix_option_model_id",
                table: "option",
                column: "model_id");

            migrationBuilder.AddForeignKey(
                name: "fk_option_model_model_id",
                table: "option",
                column: "model_id",
                principalTable: "model",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicles_brand_brand_id",
                table: "vehicles",
                column: "brand_id",
                principalTable: "brand",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicles_model_model_id",
                table: "vehicles",
                column: "model_id",
                principalTable: "model",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_option_model_model_id",
                table: "option");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicles_brand_brand_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicles_model_model_id",
                table: "vehicles");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "model");

            migrationBuilder.DropIndex(
                name: "ix_vehicles_brand_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "ix_vehicles_model_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "ix_option_model_id",
                table: "option");

            migrationBuilder.DropColumn(
                name: "brand_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "mileage_value",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "model_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "model_id",
                table: "option");
        }
    }
}
