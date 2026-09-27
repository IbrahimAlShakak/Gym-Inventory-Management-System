using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Inventory_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelWithDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumables_Item_Id",
                table: "Consumables");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Item_Id",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_Categories_CategoryId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLevels_Item_ItemId",
                table: "StockLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Item_ItemId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Item_ItemId",
                table: "StockTransfers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Item",
                table: "Item");

            migrationBuilder.RenameTable(
                name: "Item",
                newName: "Items");

            migrationBuilder.RenameIndex(
                name: "IX_Item_CategoryId",
                table: "Items",
                newName: "IX_Items_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumables_Items_Id",
                table: "Consumables",
                column: "Id",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Items_Id",
                table: "Equipment",
                column: "Id",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLevels_Items_ItemId",
                table: "StockLevels",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Items_ItemId",
                table: "StockMovements",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Items_ItemId",
                table: "StockTransfers",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumables_Items_Id",
                table: "Consumables");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Items_Id",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLevels_Items_ItemId",
                table: "StockLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Items_ItemId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Items_ItemId",
                table: "StockTransfers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "Item");

            migrationBuilder.RenameIndex(
                name: "IX_Items_CategoryId",
                table: "Item",
                newName: "IX_Item_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Item",
                table: "Item",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumables_Item_Id",
                table: "Consumables",
                column: "Id",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Item_Id",
                table: "Equipment",
                column: "Id",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Categories_CategoryId",
                table: "Item",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLevels_Item_ItemId",
                table: "StockLevels",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Item_ItemId",
                table: "StockMovements",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Item_ItemId",
                table: "StockTransfers",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
