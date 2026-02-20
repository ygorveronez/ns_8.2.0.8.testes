using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class StatusDTRequest
    {
        public string driverTicket { get; set; }
        public string dateTimeRequest { get; set; }
        public List<string> dtNumbers { get; set; }
    }
}
