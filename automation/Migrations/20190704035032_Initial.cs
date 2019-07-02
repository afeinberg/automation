using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace automation.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensor",
                columns: table => new
                {
                    Id = table.Column<decimal>(nullable: false),
                    FriendlyName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CustomData = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryData",
                columns: table => new
                {
                    Id = table.Column<decimal>(nullable: false),
                    SensorId = table.Column<decimal>(nullable: true),
                    Temperature = table.Column<double>(nullable: true),
                    Humidity = table.Column<double>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: true),
                    CustomData = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelemetryData_Sensor_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryData_SensorId",
                table: "TelemetryData",
                column: "SensorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelemetryData");

            migrationBuilder.DropTable(
                name: "Sensor");
        }
    }
}
