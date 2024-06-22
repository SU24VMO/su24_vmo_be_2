using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class ConfigPKCreateActivityRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_RequestManager_RequestManagerID",
                table: "CreateActivityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateOrganizationManagerRequest_RequestManager_RequestManagerID",
                table: "CreateOrganizationManagerRequest");

            migrationBuilder.DropIndex(
                name: "IX_CreateOrganizationManagerRequest_RequestManagerID",
                table: "CreateOrganizationManagerRequest");

            migrationBuilder.DropIndex(
                name: "IX_CreateActivityRequest_RequestManagerID",
                table: "CreateActivityRequest");

            migrationBuilder.DropColumn(
                name: "RequestManagerID",
                table: "CreateOrganizationManagerRequest");

            migrationBuilder.DropColumn(
                name: "RequestManagerID",
                table: "CreateActivityRequest");

            migrationBuilder.CreateIndex(
                name: "IX_CreateOrganizationManagerRequest_ApprovedBy",
                table: "CreateOrganizationManagerRequest",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_ApprovedBy",
                table: "CreateActivityRequest",
                column: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_RequestManager_ApprovedBy",
                table: "CreateActivityRequest",
                column: "ApprovedBy",
                principalTable: "RequestManager",
                principalColumn: "RequestManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateOrganizationManagerRequest_RequestManager_ApprovedBy",
                table: "CreateOrganizationManagerRequest",
                column: "ApprovedBy",
                principalTable: "RequestManager",
                principalColumn: "RequestManagerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_RequestManager_ApprovedBy",
                table: "CreateActivityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateOrganizationManagerRequest_RequestManager_ApprovedBy",
                table: "CreateOrganizationManagerRequest");

            migrationBuilder.DropIndex(
                name: "IX_CreateOrganizationManagerRequest_ApprovedBy",
                table: "CreateOrganizationManagerRequest");

            migrationBuilder.DropIndex(
                name: "IX_CreateActivityRequest_ApprovedBy",
                table: "CreateActivityRequest");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestManagerID",
                table: "CreateOrganizationManagerRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestManagerID",
                table: "CreateActivityRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreateOrganizationManagerRequest_RequestManagerID",
                table: "CreateOrganizationManagerRequest",
                column: "RequestManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_RequestManagerID",
                table: "CreateActivityRequest",
                column: "RequestManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_RequestManager_RequestManagerID",
                table: "CreateActivityRequest",
                column: "RequestManagerID",
                principalTable: "RequestManager",
                principalColumn: "RequestManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateOrganizationManagerRequest_RequestManager_RequestManagerID",
                table: "CreateOrganizationManagerRequest",
                column: "RequestManagerID",
                principalTable: "RequestManager",
                principalColumn: "RequestManagerID");
        }
    }
}
