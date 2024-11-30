using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autofleetapifinal.Migrations
{
    /// <inheritdoc />
    public partial class Renter : Migration
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
                name: "FK_realtime_carupdate_renter_renter_id",
                table: "realtime_carupdate");

            migrationBuilder.DropForeignKey(
                name: "FK_rented_vehicle_renter_renter_id",
                table: "rented_vehicle");

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
