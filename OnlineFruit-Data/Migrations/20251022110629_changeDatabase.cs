using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineFruit_Data.Migrations
{
    /// <inheritdoc />
    public partial class changeDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "UnitType",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UnitType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TrackingCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "NationalCode",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");
        }
    }
}
