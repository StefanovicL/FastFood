using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastFood.Migrations
{
    public partial class AddOrderLifecycleTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OrderCancelledDateTime",
                table: "Order",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderReadyDateTime",
                table: "Order",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderCancelledDateTime",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderReadyDateTime",
                table: "Order");
        }
    }
}
