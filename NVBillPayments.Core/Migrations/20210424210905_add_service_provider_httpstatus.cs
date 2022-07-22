using Microsoft.EntityFrameworkCore.Migrations;

namespace NVBillPayments.Core.Migrations
{
    public partial class add_service_provider_httpstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceProviderHTTPResponseStatusCode",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceProviderHTTPResponseStatusCode",
                table: "Transactions");
        }
    }
}
