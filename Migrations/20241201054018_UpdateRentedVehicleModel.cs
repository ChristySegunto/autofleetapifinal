using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autofleetapifinal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentedVehicleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rented_vehicle_renter_renter_id",
                table: "rented_vehicle");

            migrationBuilder.DropForeignKey(
                name: "FK_rented_vehicle_vehicle_vehicle_id",
                table: "rented_vehicle");

            migrationBuilder.DropColumn(
                name: "dropoff_loc",
                table: "rented_vehicle");

            migrationBuilder.DropColumn(
                name: "pickup_loc",
                table: "rented_vehicle");

            migrationBuilder.AlterColumn<string>(
                name: "rent_status",
                table: "rented_vehicle",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "rented_vehicle",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "rented_vehicle",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_rented_vehicle_renter_renter_id",
                table: "rented_vehicle",
                column: "renter_id",
                principalTable: "renter",
                principalColumn: "renter_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rented_vehicle_vehicle_vehicle_id",
                table: "rented_vehicle",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "vehicle_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rented_vehicle_renter_renter_id",
                table: "rented_vehicle");

            migrationBuilder.DropForeignKey(
                name: "FK_rented_vehicle_vehicle_vehicle_id",
                table: "rented_vehicle");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "rented_vehicle");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "rented_vehicle");

            migrationBuilder.AlterColumn<string>(
                name: "rent_status",
                table: "rented_vehicle",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "dropoff_loc",
                table: "rented_vehicle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "pickup_loc",
                table: "rented_vehicle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_rented_vehicle_renter_renter_id",
                table: "rented_vehicle",
                column: "renter_id",
                principalTable: "renter",
                principalColumn: "renter_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rented_vehicle_vehicle_vehicle_id",
                table: "rented_vehicle",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "vehicle_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
