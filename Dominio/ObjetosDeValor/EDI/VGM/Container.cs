using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.VGM
{
    public class Container
    {
        public string EQD { get; set; }
        public string CN { get; set; }
        public string NumeroContainer { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<PesoContainer> PesoContainer { get; set; }
    }

    public class Booking
    {
        public string RFF { get; set; }
        public string BN { get; set; }
        public string NumeroBooking { get; set; }
    }

    public class PesoContainer
    {
        public string MEA { get; set; }
        public string AAE { get; set; }
        public string VGM { get; set; }
        public string KGM { get; set; }
        public decimal Peso { get; set; }
    }
}
