using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BasicBilling.API.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceType = table.Column<string>(type: "TEXT", nullable: false),
                    Period = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bills_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BillId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 100, "Joseph Carlton" },
                    { 200, "Maria Juarez" },
                    { 300, "Albert Kenny" },
                    { 400, "Jessica Phillips" },
                    { 500, "Charles Johnson" }
                });

            migrationBuilder.InsertData(
                table: "Bills",
                columns: new[] { "Id", "Amount", "ClientId", "Period", "ServiceType", "Status" },
                values: new object[,]
                {
                    { 1, 110.22m, 100, "202602", "Water", "Pending" },
                    { 2, 31.14m, 100, "202602", "Electricity", "Pending" },
                    { 3, 28.83m, 100, "202602", "Sewer", "Pending" },
                    { 4, 88.41m, 100, "202603", "Water", "Pending" },
                    { 5, 35.27m, 100, "202603", "Electricity", "Pending" },
                    { 6, 49.39m, 100, "202603", "Sewer", "Pending" },
                    { 7, 118.66m, 200, "202602", "Water", "Pending" },
                    { 8, 86.94m, 200, "202602", "Electricity", "Pending" },
                    { 9, 36.05m, 200, "202602", "Sewer", "Pending" },
                    { 10, 124.19m, 200, "202603", "Water", "Pending" },
                    { 11, 45.19m, 200, "202603", "Electricity", "Pending" },
                    { 12, 48.60m, 200, "202603", "Sewer", "Pending" },
                    { 13, 85.84m, 300, "202602", "Water", "Pending" },
                    { 14, 58.03m, 300, "202602", "Electricity", "Pending" },
                    { 15, 67.15m, 300, "202602", "Sewer", "Pending" },
                    { 16, 49.04m, 300, "202603", "Water", "Pending" },
                    { 17, 87.62m, 300, "202603", "Electricity", "Pending" },
                    { 18, 15.30m, 300, "202603", "Sewer", "Pending" },
                    { 19, 132.12m, 400, "202602", "Water", "Pending" },
                    { 20, 96.58m, 400, "202602", "Electricity", "Pending" },
                    { 21, 69.62m, 400, "202602", "Sewer", "Pending" },
                    { 22, 32.83m, 400, "202603", "Water", "Pending" },
                    { 23, 23.52m, 400, "202603", "Electricity", "Pending" },
                    { 24, 115.77m, 400, "202603", "Sewer", "Pending" },
                    { 25, 132.43m, 500, "202602", "Water", "Pending" },
                    { 26, 91.00m, 500, "202602", "Electricity", "Pending" },
                    { 27, 16.62m, 500, "202602", "Sewer", "Pending" },
                    { 28, 116.67m, 500, "202603", "Water", "Pending" },
                    { 29, 32.16m, 500, "202603", "Electricity", "Pending" },
                    { 30, 145.93m, 500, "202603", "Sewer", "Pending" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bills_ClientId",
                table: "Bills",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillId",
                table: "Payments",
                column: "BillId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
