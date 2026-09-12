using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointmentSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addMedicine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    Medicine_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.Medicine_Id);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionDetails",
                columns: table => new
                {
                    Prescription_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Appointment_Id = table.Column<int>(type: "int", nullable: false),
                    Medicine_Id = table.Column<int>(type: "int", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Start_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionDetails", x => x.Prescription_Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionDetails_Appointments_Appointment_Id",
                        column: x => x.Appointment_Id,
                        principalTable: "Appointments",
                        principalColumn: "Appointment_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrescriptionDetails_Medicines_Medicine_Id",
                        column: x => x.Medicine_Id,
                        principalTable: "Medicines",
                        principalColumn: "Medicine_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionDetails_Appointment_Id",
                table: "PrescriptionDetails",
                column: "Appointment_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionDetails_Medicine_Id",
                table: "PrescriptionDetails",
                column: "Medicine_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrescriptionDetails");

            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Prescription_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Appointment_Id = table.Column<int>(type: "int", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    End_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Medicine = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Start_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Prescription_Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Appointments_Appointment_Id",
                        column: x => x.Appointment_Id,
                        principalTable: "Appointments",
                        principalColumn: "Appointment_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_Appointment_Id",
                table: "Prescriptions",
                column: "Appointment_Id");
        }
    }
}
