using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class AddIsDisableField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisable",
                table: "Post",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisable",
                table: "Organization",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisable",
                table: "CreateVolunteerRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisable",
                table: "CreateOrganizationManagerRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisable",
                table: "Campaign",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisable",
                table: "Activity",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisable",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "IsDisable",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "IsDisable",
                table: "CreateVolunteerRequest");

            migrationBuilder.DropColumn(
                name: "IsDisable",
                table: "CreateOrganizationManagerRequest");

            migrationBuilder.DropColumn(
                name: "IsDisable",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "IsDisable",
                table: "Activity");
        }
    }
}
