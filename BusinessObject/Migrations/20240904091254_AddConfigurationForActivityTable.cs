using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class AddConfigurationForActivityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityStatementFile_Activity_ActivityId",
                table: "ActivityStatementFile");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityStatementFile_Activity_ActivityId",
                table: "ActivityStatementFile",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityStatementFile_Activity_ActivityId",
                table: "ActivityStatementFile");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityStatementFile_Activity_ActivityId",
                table: "ActivityStatementFile",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
