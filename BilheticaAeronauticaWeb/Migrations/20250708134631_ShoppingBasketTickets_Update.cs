using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilheticaAeronauticaWeb.Migrations
{
    /// <inheritdoc />
    public partial class ShoppingBasketTickets_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponsibleAdultId",
                table: "ShoppingBasketTickets",
                newName: "ResponsibleAdultTicketId");

            migrationBuilder.AddColumn<bool>(
                name: "IsResponsibleAdult",
                table: "ShoppingBasketTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResponsibleAdult",
                table: "ShoppingBasketTickets");

            migrationBuilder.RenameColumn(
                name: "ResponsibleAdultTicketId",
                table: "ShoppingBasketTickets",
                newName: "ResponsibleAdultId");
        }
    }
}
