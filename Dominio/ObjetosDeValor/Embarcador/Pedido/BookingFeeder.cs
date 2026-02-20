using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class BookingFeeder
    {
        public string NumeroBooking { get; set; }
        public int QtdPlanilha { get; set; }
        public int QtdSistema { get; set; }
        public List<BookingFeederContainer> Containeres { get; set; }
    }
}
