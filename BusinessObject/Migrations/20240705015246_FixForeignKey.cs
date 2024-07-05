using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class FixForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_Member_MemberID",
                table: "CreateActivityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_OrganizationManager_OrganizationManagerID",
                table: "CreateActivityRequest");

            migrationBuilder.DropIndex(
                name: "IX_CreateActivityRequest_MemberID",
                table: "CreateActivityRequest");

            migrationBuilder.DropIndex(
                name: "IX_CreateActivityRequest_OrganizationManagerID",
                table: "CreateActivityRequest");

            migrationBuilder.DropColumn(
                name: "MemberID",
                table: "CreateActivityRequest");

            migrationBuilder.DropColumn(
                name: "OrganizationManagerID",
                table: "CreateActivityRequest");

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_CreateByMember",
                table: "CreateActivityRequest",
                column: "CreateByMember");

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_CreateByOM",
                table: "CreateActivityRequest",
                column: "CreateByOM");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_Member_CreateByMember",
                table: "CreateActivityRequest",
                column: "CreateByMember",
                principalTable: "Member",
                principalColumn: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_OrganizationManager_CreateByOM",
                table: "CreateActivityRequest",
                column: "CreateByOM",
                principalTable: "OrganizationManager",
                principalColumn: "OrganizationManagerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_Member_CreateByMember",
                table: "CreateActivityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_OrganizationManager_CreateByOM",
                table: "CreateActivityRequest");

            migrationBuilder.DropIndex(
                name: "IX_CreateActivityRequest_CreateByMember",
                table: "CreateActivityRequest");

            migrationBuilder.DropIndex(
                name: "IX_CreateActivityRequest_CreateByOM",
                table: "CreateActivityRequest");

            migrationBuilder.AddColumn<Guid>(
                name: "MemberID",
                table: "CreateActivityRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationManagerID",
                table: "CreateActivityRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_MemberID",
                table: "CreateActivityRequest",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_OrganizationManagerID",
                table: "CreateActivityRequest",
                column: "OrganizationManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_Member_MemberID",
                table: "CreateActivityRequest",
                column: "MemberID",
                principalTable: "Member",
                principalColumn: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_OrganizationManager_OrganizationManagerID",
                table: "CreateActivityRequest",
                column: "OrganizationManagerID",
                principalTable: "OrganizationManager",
                principalColumn: "OrganizationManagerID");
        }
    }
}
