using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilheticaAeronauticaWeb.Migrations
{
    /// <inheritdoc />
    public partial class Update_Remove_ShoppingBaskets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingBasketTickets_ShoppingBaskets_ShoppingBasketId",
                table: "ShoppingBasketTickets");

            migrationBuilder.DropTable(
                name: "ShoppingBaskets");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingBasketTickets_ShoppingBasketId",
                table: "ShoppingBasketTickets");

            migrationBuilder.DropColumn(
                name: "ShoppingBasketId",
                table: "ShoppingBasketTickets");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ShoppingBasketTickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBasketTickets_UserId",
                table: "ShoppingBasketTickets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingBasketTickets_AspNetUsers_UserId",
                table: "ShoppingBasketTickets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingBasketTickets_AspNetUsers_UserId",
                table: "ShoppingBasketTickets");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingBasketTickets_UserId",
                table: "ShoppingBasketTickets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShoppingBasketTickets");

            migrationBuilder.AddColumn<int>(
                name: "ShoppingBasketId",
                table: "ShoppingBasketTickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ShoppingBaskets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingBaskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingBaskets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBasketTickets_ShoppingBasketId",
                table: "ShoppingBasketTickets",
                column: "ShoppingBasketId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBaskets_UserId",
                table: "ShoppingBaskets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingBasketTickets_ShoppingBaskets_ShoppingBasketId",
                table: "ShoppingBasketTickets",
                column: "ShoppingBasketId",
                principalTable: "ShoppingBaskets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
