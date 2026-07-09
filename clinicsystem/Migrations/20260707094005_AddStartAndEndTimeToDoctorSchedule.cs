using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace clinicsystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStartAndEndTimeToDoctorSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Doctors_DoctorId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Patients_PatientId",
                table: "Reservations");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "DoctorSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "DoctorSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Doctors_DoctorId",
                table: "Reservations",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Patients_PatientId",
                table: "Reservations",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Doctors_DoctorId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Patients_PatientId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "DoctorSchedules");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Doctors_DoctorId",
                table: "Reservations",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Patients_PatientId",
                table: "Reservations",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
