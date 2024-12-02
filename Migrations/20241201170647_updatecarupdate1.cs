using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autofleetapifinal.Migrations
{
    /// <inheritdoc />
    public partial class updatecarupdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_realtime_carupdate_renter_renter_id",
                table: "realtime_carupdate");

            migrationBuilder.DropForeignKey(
                name: "FK_rented_vehicle_renter_renter_id",
                table: "rented_vehicle");

            migrationBuilder.DropForeignKey(
                name: "FK_renter_Users_user_id",
                table: "renter");

            migrationBuilder.DropIndex(
                name: "IX_renter_user_id",
                table: "renter");

            migrationBuilder.DropColumn(
                name: "dropoff_loc",
                table: "rented_vehicle");

            migrationBuilder.DropColumn(
                name: "pickup_loc",
                table: "rented_vehicle");

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

            migrationBuilder.AddColumn<int>(
                name: "rented_vehicle_id",
                table: "realtime_carupdate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_realtime_carupdate_rented_vehicle_renter_id",
                table: "realtime_carupdate",
                column: "renter_id",
                principalTable: "rented_vehicle",
                principalColumn: "rented_vehicle_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_realtime_carupdate_renter_renter_id",
                table: "realtime_carupdate",
                column: "renter_id",
                principalTable: "renter",
                principalColumn: "renter_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rented_vehicle_renter_renter_id",
                table: "rented_vehicle",
                column: "renter_id",
                principalTable: "renter",
                principalColumn: "renter_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_realtime_carupdate_rented_vehicle_renter_id",
                table: "realtime_carupdate");

            migrationBuilder.DropForeignKey(
                name: "FK_realtime_carupdate_renter_renter_id",
                table: "realtime_carupdate");

            migrationBuilder.DropForeignKey(
                name: "FK_rented_vehicle_renter_renter_id",
                table: "rented_vehicle");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "rented_vehicle");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "rented_vehicle");

            migrationBuilder.DropColumn(
                name: "rented_vehicle_id",
                table: "realtime_carupdate");

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

            migrationBuilder.CreateIndex(
                name: "IX_renter_user_id",
                table: "renter",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_realtime_carupdate_renter_renter_id",
                table: "realtime_carupdate",
                column: "renter_id",
                principalTable: "renter",
                principalColumn: "renter_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rented_vehicle_renter_renter_id",
                table: "rented_vehicle",
                column: "renter_id",
                principalTable: "renter",
                principalColumn: "renter_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_renter_Users_user_id",
                table: "renter",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
