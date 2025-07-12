using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilheticaAeronauticaWeb.Migrations
{
    /// <inheritdoc />
    public partial class Responsible_Adult_Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponsibleAdultId",
                table: "Tickets",
                newName: "ResponsibleAdultTicketId");

            migrationBuilder.AddColumn<bool>(
                name: "IsResponsibleAdult",
                table: "Tickets",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBasketTickets_FlightId",
                table: "ShoppingBasketTickets",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBasketTickets_SeatId",
                table: "ShoppingBasketTickets",
                column: "SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingBasketTickets_Flights_FlightId",
                table: "ShoppingBasketTickets",
                column: "FlightId",
                principalTable: "Flights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingBasketTickets_Seats_SeatId",
                table: "ShoppingBasketTickets",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingBasketTickets_Flights_FlightId",
                table: "ShoppingBasketTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingBasketTickets_Seats_SeatId",
                table: "ShoppingBasketTickets");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingBasketTickets_FlightId",
                table: "ShoppingBasketTickets");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingBasketTickets_SeatId",
                table: "ShoppingBasketTickets");

            migrationBuilder.DropColumn(
                name: "IsResponsibleAdult",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "ResponsibleAdultTicketId",
                table: "Tickets",
                newName: "ResponsibleAdultId");
        }
    }
}
