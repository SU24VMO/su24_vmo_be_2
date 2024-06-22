using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCreateOrganizationRequestField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrganizationManagerTaxCode",
                table: "CreateOrganizationRequest",
                newName: "OrganizationTaxCode");

            migrationBuilder.AddColumn<string>(
                name: "AuthorizationDocuments",
                table: "CreateOrganizationRequest",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizationDocuments",
                table: "CreateOrganizationRequest");

            migrationBuilder.RenameColumn(
                name: "OrganizationTaxCode",
                table: "CreateOrganizationRequest",
                newName: "OrganizationManagerTaxCode");
        }
    }
}
