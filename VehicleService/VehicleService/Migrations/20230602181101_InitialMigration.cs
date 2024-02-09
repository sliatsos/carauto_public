using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuto.VehicleService.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    vehicle_type = table.Column<int>(type: "integer", nullable: false),
                    vin = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    license_no = table.Column<string>(type: "text", nullable: false),
                    registration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    vehicle_status = table.Column<int>(type: "integer", nullable: false),
                    business_partner_type = table.Column<int>(type: "integer", nullable: false),
                    business_partner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mileage_unit_of_measure = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "option",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    internal_code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<double>(type: "double precision", nullable: false),
                    unit_cost = table.Column<double>(type: "double precision", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_option", x => x.id);
                    table.ForeignKey(
                        name: "fk_option_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_option_vehicle_id",
                table: "option",
                column: "vehicle_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "option");

            migrationBuilder.DropTable(
                name: "vehicles");
        }
    }
}
