using System;

namespace Dominio.ObjetosDeValor.WebService.Carga.BookingAVRO
{
    public class LegBooking
    {
        public int id { get; set; }
        public int legNumber { get; set; }
        public string vesselImoNumber { get; set; }
        public string callSign { get; set; }
        public VesselName vesselName { get; set; }
        public string vesselShortName { get; set; }
        public string voyageAndDirection { get; set; }
        public string vvdConnection { get; set; }
        public Port portOrigin { get; set; }
        public Port portTerminalOrigin { get; set; }
        public Port portDestination { get; set; }
        public Port portTerminalDestination { get; set; }
        public DateTime estimateArrival { get; set; }
        public DateTime estimateDeparture { get; set; }
    }
}
