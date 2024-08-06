using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class FixForeignKeyOfCampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessingPhase_Campaign_CampaignId",
                table: "ProcessingPhase");

            migrationBuilder.DropIndex(
                name: "IX_ProcessingPhase_CampaignId",
                table: "ProcessingPhase");

            migrationBuilder.AddColumn<string>(
                name: "CurrentMoney",
                table: "ProcessingPhase",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProcessingPhase",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Percent",
                table: "ProcessingPhase",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CampaignTier",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingPhase_CampaignId",
                table: "ProcessingPhase",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessingPhase_Campaign_CampaignId",
                table: "ProcessingPhase",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "CampaignID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessingPhase_Campaign_CampaignId",
                table: "ProcessingPhase");

            migrationBuilder.DropIndex(
                name: "IX_ProcessingPhase_CampaignId",
                table: "ProcessingPhase");

            migrationBuilder.DropColumn(
                name: "CurrentMoney",
                table: "ProcessingPhase");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProcessingPhase");

            migrationBuilder.DropColumn(
                name: "Percent",
                table: "ProcessingPhase");

            migrationBuilder.DropColumn(
                name: "CampaignTier",
                table: "Campaign");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingPhase_CampaignId",
                table: "ProcessingPhase",
                column: "CampaignId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessingPhase_Campaign_CampaignId",
                table: "ProcessingPhase",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "CampaignID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
