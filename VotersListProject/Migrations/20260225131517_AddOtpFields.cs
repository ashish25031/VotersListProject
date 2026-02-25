using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotersListProject.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailOTP",
                table: "votertable",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "votertable",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPExpiry",
                table: "votertable",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailOTP",
                table: "votertable");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "votertable");

            migrationBuilder.DropColumn(
                name: "OTPExpiry",
                table: "votertable");
        }
    }
}
