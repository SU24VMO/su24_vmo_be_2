using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class AddBankingAccountForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CampaignId",
                table: "BankingAccount",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BankingAccount_CampaignId",
                table: "BankingAccount",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankingAccount_Campaign_CampaignId",
                table: "BankingAccount",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "CampaignID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankingAccount_Campaign_CampaignId",
                table: "BankingAccount");

            migrationBuilder.DropIndex(
                name: "IX_BankingAccount_CampaignId",
                table: "BankingAccount");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "BankingAccount");
        }
    }
}
