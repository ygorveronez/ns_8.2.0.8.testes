using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class StatusDTResponse
    {
        public string driverTicket { get; set; }
        public string dateTimeRequest { get; set; }
        public List<StatusDT> dtStatus { get; set; }
    }
}
