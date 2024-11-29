using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autofleetapifinal.Migrations
{
    /// <inheritdoc />
    public partial class InitialChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maintenance",
                columns: table => new
                {
                    maintenance_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vehicle_id = table.Column<int>(type: "int", nullable: false),
                    plate_num = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    car_model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    maintenance_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    maintenance_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    maintenance_due_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    maintenance_next_due_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maintenance", x => x.maintenance_id);
                });

            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    report_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    renter_id = table.Column<int>(type: "int", nullable: false),
                    nature_of_issue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    time = table.Column<TimeSpan>(type: "time", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emergency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report", x => x.report_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle",
                columns: table => new
                {
                    vehicle_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    car_manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    car_model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    plate_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    manufacture_year = table.Column<int>(type: "int", nullable: true),
                    vehicle_color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fuel_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transmission_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    seating_capacity = table.Column<int>(type: "int", nullable: true),
                    vehicle_category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    total_mileage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    total_fuel_consumption = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    distance_traveled = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    vehicle_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle", x => x.vehicle_id);
                });

            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    admin_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    admin_fname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    admin_mname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    admin_lname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    admin_birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin", x => x.admin_id);
                    table.ForeignKey(
                        name: "FK_admin_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "renter",
                columns: table => new
                {
                    renter_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    renter_fname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    renter_mname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    renter_lname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    renter_birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    renter_contact_num = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    renter_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    renter_emergency_contact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    renter_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    renter_id_photo_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    renter_id_photo_2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_renter", x => x.renter_id);
                    table.ForeignKey(
                        name: "FK_renter_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "realtime_carupdate",
                columns: table => new
                {
                    carupdate_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    renter_id = table.Column<int>(type: "int", nullable: false),
                    renter_fname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    renter_lname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    location_latitude = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    location_longitude = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    speed = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    total_fuel_consumption = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    total_distance_travelled = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    vehicle_id = table.Column<int>(type: "int", nullable: false),
                    carupdate_status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_realtime_carupdate", x => x.carupdate_id);
                    table.ForeignKey(
                        name: "FK_realtime_carupdate_renter_renter_id",
                        column: x => x.renter_id,
                        principalTable: "renter",
                        principalColumn: "renter_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rented_vehicle",
                columns: table => new
                {
                    rented_vehicle_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    renter_id = table.Column<int>(type: "int", nullable: false),
                    vehicle_id = table.Column<int>(type: "int", nullable: false),
                    renter_fname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    renter_lname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pickup_loc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pickup_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    pickup_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    dropoff_loc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dropoff_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dropoff_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    car_manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    car_model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    plate_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rent_status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rented_vehicle", x => x.rented_vehicle_id);
                    table.ForeignKey(
                        name: "FK_rented_vehicle_renter_renter_id",
                        column: x => x.renter_id,
                        principalTable: "renter",
                        principalColumn: "renter_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rented_vehicle_vehicle_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "vehicle_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_user_id",
                table: "admin",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_realtime_carupdate_renter_id",
                table: "realtime_carupdate",
                column: "renter_id");

            migrationBuilder.CreateIndex(
                name: "IX_rented_vehicle_renter_id",
                table: "rented_vehicle",
                column: "renter_id");

            migrationBuilder.CreateIndex(
                name: "IX_rented_vehicle_vehicle_id",
                table: "rented_vehicle",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_renter_user_id",
                table: "renter",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "maintenance");

            migrationBuilder.DropTable(
                name: "realtime_carupdate");

            migrationBuilder.DropTable(
                name: "rented_vehicle");

            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropTable(
                name: "renter");

            migrationBuilder.DropTable(
                name: "vehicle");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
