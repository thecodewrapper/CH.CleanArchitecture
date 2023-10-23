using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CH.CleanArchitecture.Infrastructure.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddedNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                schema: "Domain",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                schema: "Domain",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BillingAddressId",
                schema: "Domain",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingAddressId",
                schema: "Domain",
                table: "Orders");

            migrationBuilder.EnsureSchema(
                name: "App");

            migrationBuilder.AlterColumn<Guid>(
                name: "ShippingAddressId",
                schema: "Domain",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BillingAddressId",
                schema: "Domain",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "App",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    IsNew = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BillingAddressId",
                schema: "Domain",
                table: "Orders",
                column: "BillingAddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddressId",
                schema: "Domain",
                table: "Orders",
                column: "ShippingAddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                schema: "Domain",
                table: "Orders",
                column: "BillingAddressId",
                principalSchema: "Domain",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                schema: "Domain",
                table: "Orders",
                column: "ShippingAddressId",
                principalSchema: "Domain",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                schema: "Domain",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                schema: "Domain",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "App");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BillingAddressId",
                schema: "Domain",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingAddressId",
                schema: "Domain",
                table: "Orders");

            migrationBuilder.AlterColumn<Guid>(
                name: "ShippingAddressId",
                schema: "Domain",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BillingAddressId",
                schema: "Domain",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BillingAddressId",
                schema: "Domain",
                table: "Orders",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddressId",
                schema: "Domain",
                table: "Orders",
                column: "ShippingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                schema: "Domain",
                table: "Orders",
                column: "BillingAddressId",
                principalSchema: "Domain",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                schema: "Domain",
                table: "Orders",
                column: "ShippingAddressId",
                principalSchema: "Domain",
                principalTable: "Addresses",
                principalColumn: "Id");
        }
    }
}
