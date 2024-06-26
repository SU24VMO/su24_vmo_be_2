using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class AddIsLockField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "StatementPhase",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateBy",
                table: "StatementPhase",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "ProcessingPhase",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateBy",
                table: "ProcessingPhase",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "DonatePhase",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateBy",
                table: "DonatePhase",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "StatementPhase");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "StatementPhase");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "ProcessingPhase");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "ProcessingPhase");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "DonatePhase");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "DonatePhase");
        }
    }
}
