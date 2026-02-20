using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.GTA
{
    public class SUINO
    {
        public string Numero { get; set; }
        public int Protocolo { get; set; }
        public string HorarioInicioViagem { get; set; }
        public string HorarioTerminoViagem { get; set; }
        public List<Parada> Paradas { get; set; }
    }
}