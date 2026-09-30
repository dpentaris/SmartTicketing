using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerryBooking.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vessels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PassengerCapacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vessels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sailings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginPortId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationPortId = table.Column<Guid>(type: "uuid", nullable: false),
                    VesselId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartureTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ArrivalTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FarePerPassenger = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    SeatsReserved = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sailings", x => x.Id);
                    table.CheckConstraint("CK_Sailings_DifferentPorts", "\"OriginPortId\" <> \"DestinationPortId\"");
                    table.CheckConstraint("CK_Sailings_SeatsWithinCapacity", "\"SeatsReserved\" BETWEEN 0 AND \"Capacity\"");
                    table.ForeignKey(
                        name: "FK_Sailings_Ports_DestinationPortId",
                        column: x => x.DestinationPortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sailings_Ports_OriginPortId",
                        column: x => x.OriginPortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sailings_Vessels_VesselId",
                        column: x => x.VesselId,
                        principalTable: "Vessels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ports_Code",
                table: "Ports",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sailings_DestinationPortId",
                table: "Sailings",
                column: "DestinationPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Sailings_OriginPortId_DestinationPortId_DepartureTime",
                table: "Sailings",
                columns: new[] { "OriginPortId", "DestinationPortId", "DepartureTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Sailings_VesselId",
                table: "Sailings",
                column: "VesselId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sailings");

            migrationBuilder.DropTable(
                name: "Ports");

            migrationBuilder.DropTable(
                name: "Vessels");
        }
    }
}
