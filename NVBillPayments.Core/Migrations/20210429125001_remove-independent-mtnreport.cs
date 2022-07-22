using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NVBillPayments.Core.Migrations
{
    public partial class removeindependentmtnreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MTNBundleReports");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MTNBundleReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivationChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountCharged = table.Column<decimal>(type: "decimal(15,3)", nullable: false),
                    BeneficiaryMSISDN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BundlePriceUGX = table.Column<decimal>(type: "decimal(15,3)", nullable: false),
                    BundleValidity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HttpStatusCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseBodyStatusCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionTimeUgandanTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MTNBundleReports", x => x.Id);
                });
        }
    }
}
