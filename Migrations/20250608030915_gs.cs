using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexus_response.Migrations
{
    /// <inheritdoc />
    public partial class gs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    LastCommunication = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(11)", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    StatusReport = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    UrgencyLevel = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    UrgencyScore = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UrgencyFactors = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ReportedById = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidents_Users_ReportedById",
                        column: x => x.ReportedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidentComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Content = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UserName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    IncidentId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentComments_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IoTData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DeviceId = table.Column<int>(type: "NUMBER(10)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    RawData = table.Column<string>(type: "NCLOB", nullable: false),
                    Temperature = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Humidity = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Distance = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    AccelerometerX = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    AccelerometerY = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    AccelerometerZ = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IncidentId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IoTData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IoTData_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IoTData_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentComments_IncidentId",
                table: "IncidentComments",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_ReportedById",
                table: "Incidents",
                column: "ReportedById");

            migrationBuilder.CreateIndex(
                name: "IX_IoTData_DeviceId",
                table: "IoTData",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_IoTData_IncidentId",
                table: "IoTData",
                column: "IncidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentComments");

            migrationBuilder.DropTable(
                name: "IoTData");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Incidents");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
