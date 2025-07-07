namespace BilheticaAeronauticaWeb.Entities
{
    public class Seat : IEntity
    {
        public int Id { get; set ; }

        public int FlightId { get; set; }

        public Flight Flight { get; set ; }

        public bool Occupied { get; set ; }

        public int Row { get; set ; }

        public int Column { get; set ; }

        public DateTime? HoldingTime { get; set; }

        public bool IsHeld { get; set; }
    }
}
