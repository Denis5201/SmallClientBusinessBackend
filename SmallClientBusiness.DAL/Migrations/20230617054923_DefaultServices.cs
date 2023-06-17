using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmallClientBusiness.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DefaultServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Duration", "Name", "Price", "WorkerId" },
                values: new object[,]
                {
                    { new Guid("0cdb988c-4b9f-4713-805f-864a9c7563ac"), new TimeOnly(2, 0, 0), "Наращивание ногтей", 1000.0, null },
                    { new Guid("146ff8c8-b942-44ae-8e1e-cc658ec47bad"), new TimeOnly(0, 50, 0), "Ламинирование бровей", 600.0, null },
                    { new Guid("20a90b4c-ceaa-4ae5-8502-c20114425150"), new TimeOnly(0, 40, 0), "Массаж", 800.0, null },
                    { new Guid("3f044dec-0643-43b8-a3da-3e8b7749d665"), new TimeOnly(1, 0, 0), "Маникюр", 1200.0, null },
                    { new Guid("b1942411-2a3f-416b-bc3d-9f22090b02f5"), new TimeOnly(1, 15, 0), "Окрашивание", 2500.0, null },
                    { new Guid("d6450d0d-c6df-450a-9534-8f843ec73eb7"), new TimeOnly(0, 45, 0), "Укладка волос", 400.0, null },
                    { new Guid("fb767e25-05c7-4a10-bec8-08f669eb16d3"), new TimeOnly(1, 10, 0), "Наращивание ресниц", 1400.0, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("0cdb988c-4b9f-4713-805f-864a9c7563ac"));

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("146ff8c8-b942-44ae-8e1e-cc658ec47bad"));

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("20a90b4c-ceaa-4ae5-8502-c20114425150"));

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("3f044dec-0643-43b8-a3da-3e8b7749d665"));

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("b1942411-2a3f-416b-bc3d-9f22090b02f5"));

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("d6450d0d-c6df-450a-9534-8f843ec73eb7"));

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("fb767e25-05c7-4a10-bec8-08f669eb16d3"));
        }
    }
}
