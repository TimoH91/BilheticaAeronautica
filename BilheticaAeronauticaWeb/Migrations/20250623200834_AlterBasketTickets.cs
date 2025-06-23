using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilheticaAeronauticaWeb.Migrations
{
    /// <inheritdoc />
    public partial class AlterBasketTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "TicketClass",
                table: "ShoppingBasketTickets",
                newName: "PassengerType");

            migrationBuilder.RenameColumn(
                name: "Passenger",
                table: "ShoppingBasketTickets",
                newName: "Class");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PassengerType",
                table: "ShoppingBasketTickets",
                newName: "TicketClass");

            migrationBuilder.RenameColumn(
                name: "Class",
                table: "ShoppingBasketTickets",
                newName: "Passenger");

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
    }
}
