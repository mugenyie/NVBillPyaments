using Microsoft.EntityFrameworkCore.Migrations;

namespace NVBillPayments.Core.Migrations
{
    public partial class addqrcodefield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QRCodeUrl",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodeUrl",
                table: "Transactions");
        }
    }
}
