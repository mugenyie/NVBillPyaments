using Microsoft.EntityFrameworkCore.Migrations;

namespace NVBillPayments.Core.Migrations
{
    public partial class addCashback_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Charge",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Commission",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Discount",
                table: "Transactions",
                newName: "Cashback");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountToCharge",
                table: "Transactions",
                type: "decimal(15,3)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountToCharge",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Cashback",
                table: "Transactions",
                newName: "Discount");

            migrationBuilder.AddColumn<float>(
                name: "Charge",
                table: "Transactions",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Commission",
                table: "Transactions",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
