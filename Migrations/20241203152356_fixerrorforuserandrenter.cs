using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autofleetapifinal.Migrations
{
    /// <inheritdoc />
    public partial class fixerrorforuserandrenter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_renter_Users_user_id1",
                table: "renter");

            migrationBuilder.DropIndex(
                name: "IX_renter_user_id1",
                table: "renter");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "renter");

            migrationBuilder.CreateIndex(
                name: "IX_renter_user_id",
                table: "renter",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_renter_Users_user_id",
                table: "renter",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_renter_Users_user_id",
                table: "renter");

            migrationBuilder.DropIndex(
                name: "IX_renter_user_id",
                table: "renter");

            migrationBuilder.AddColumn<int>(
                name: "user_id1",
                table: "renter",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_renter_user_id1",
                table: "renter",
                column: "user_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_renter_Users_user_id1",
                table: "renter",
                column: "user_id1",
                principalTable: "Users",
                principalColumn: "user_id");
        }
    }
}
