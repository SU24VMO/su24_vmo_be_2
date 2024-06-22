using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class AddModifiedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "CreatePostRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "CreateOrganizationRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "CreateOrganizationManagerRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "CreateMemberRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "CreateCampaignRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "CreateActivityRequest",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "CreatePostRequest");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "CreateOrganizationRequest");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "CreateOrganizationManagerRequest");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "CreateMemberRequest");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "CreateCampaignRequest");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "CreateActivityRequest");
        }
    }
}
