using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class FixImageTransactionField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionQRImageUrl",
                table: "Transaction",
                newName: "TransactionImageUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionImageUrl",
                table: "Transaction",
                newName: "TransactionQRImageUrl");
        }
    }
}
