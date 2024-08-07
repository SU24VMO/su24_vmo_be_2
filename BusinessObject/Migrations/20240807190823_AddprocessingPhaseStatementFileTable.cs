using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class AddprocessingPhaseStatementFileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessingPhaseStatementFile",
                columns: table => new
                {
                    ProcessingPhaseStatementFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessingPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingPhaseStatementFile", x => x.ProcessingPhaseStatementFileId);
                    table.ForeignKey(
                        name: "FK_ProcessingPhaseStatementFile_ProcessingPhase_ProcessingPhaseId",
                        column: x => x.ProcessingPhaseId,
                        principalTable: "ProcessingPhase",
                        principalColumn: "ProcessingPhaseId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingPhaseStatementFile_ProcessingPhaseId",
                table: "ProcessingPhaseStatementFile",
                column: "ProcessingPhaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessingPhaseStatementFile");
        }
    }
}
